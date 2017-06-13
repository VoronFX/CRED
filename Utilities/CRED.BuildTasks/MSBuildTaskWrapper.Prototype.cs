﻿

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using CRED.BuildTasks.Wrapper;

namespace CRED.BuildTasks
{
	namespace Prototype
	{
		public partial class TaskWrapperBase : Task
		{
			public const string TasksNodeName = "TaskRunner";
			public const string LogNodeName = "Log";

			[Required]
			[ExpandPath]
			[XmlElement]
			public string TaskAssemblyPath { get; set; }

			[ExpandPath]
			[EnsureDirectoryCreated]
			[XmlElement]
			public string IncrementalBuildCacheFile { get; set; }

			[XmlElement]
			[NormalizeDirectoryPath]
			public string RelativeRoot { get; set; }

			public static XmlSerializer ExplicitOnlySerializer(Type type)
			{
				var overrides = new XmlAttributeOverrides();
				var attributes = new XmlAttributes { XmlIgnore = true };
				foreach (var prop in type.GetProperties()
					.Where(x => !x.GetCustomAttributes(typeof(XmlElementAttribute))
						.Any()))
				{
					overrides.Add(prop.DeclaringType, prop.Name, attributes);
				}
				return new XmlSerializer(type, overrides);
			}

			[AttributeUsage(AttributeTargets.Property)]
			public sealed class EnsureDirectoryCreatedAttribute : Attribute
			{
			}

			[AttributeUsage(AttributeTargets.Property)]
			public sealed class ExpandPathAttribute : Attribute
			{
			}

			[AttributeUsage(AttributeTargets.Property)]
			public sealed class NormalizeDirectoryPathAttribute : Attribute
			{
			}

			private void ProcessPathProperties(Type attributeSelector, Func<string, string> process)
			{
				foreach (var property in GetType().GetRuntimeProperties()
					.Where(x => x.CustomAttributes
						.Any(a => a.AttributeType == attributeSelector)))
				{
					if (property.PropertyType == typeof(string))
					{
						var path = (string)property.GetValue(this);
						if (!string.IsNullOrWhiteSpace(path))
							property.SetValue(this, process(path));
					}
					else if (property.PropertyType == typeof(string[]))
					{
						var array = (string[])property.GetValue(this);
						for (var i = 0; i < array.Length; i++)
						{
							if (!string.IsNullOrWhiteSpace(array[i]))
								array[i] = process(array[i]);
						}
					}
				}
			}

			public string Serialize()
			{
				var parameters = new StringBuilder();
				using (var xmlWriter = XmlWriter.Create(parameters, new XmlWriterSettings()
				{
					Indent = true,
				}))
				{
					ExplicitOnlySerializer(GetType()).Serialize(xmlWriter, this);
					return parameters.ToString();
				}
			}

			public override bool Execute()
			{
				var success = true;
				try
				{
					ProcessPathProperties(typeof(ExpandPathAttribute),
						path => Path.GetFullPath(Path.Combine(RelativeRoot, path)));

					ProcessPathProperties(typeof(NormalizeDirectoryPathAttribute),
						path => Path.GetFullPath(path + Path.DirectorySeparatorChar));

					ProcessPathProperties(typeof(EnsureDirectoryCreatedAttribute),
						path =>
						{
							Directory.CreateDirectory(Path.GetDirectoryName(path));
							return path;
						});

					var process = new Process
					{
						StartInfo =
						{
							CreateNoWindow = true,
							UseShellExecute = false,
							RedirectStandardOutput = true,
							RedirectStandardInput = true,
							FileName = "dotnet",
							Arguments = TaskAssemblyPath
						}
					};
					process.Start();
					using (var stream = process.StandardInput)
					using (var xmlWriter = XmlWriter.Create(stream))
					{
						xmlWriter.WriteStartDocument();
						xmlWriter.WriteStartElement(TasksNodeName);
						ExplicitOnlySerializer(GetType()).Serialize(xmlWriter, this);
						xmlWriter.WriteEndElement();
						xmlWriter.WriteEndDocument();
					}
					using (var reader = XmlReader.Create(process.StandardOutput))
					{
						reader.ReadStartElement(LogNodeName);
						var serializer = new XmlSerializer(typeof(LogEvent));
						while (serializer.CanDeserialize(reader))
						{
							var logEvent = (LogEvent)serializer.Deserialize(reader);
							if (logEvent == null) continue;
							switch (logEvent.Category)
							{
								case LogCategory.Error:
									Log.LogError(logEvent.ToString());
									success = false;
									break;
								case LogCategory.Warning:
									Log.LogWarning(logEvent.ToString());
									break;
								case LogCategory.Message:
									Log.LogMessage(logEvent.ToString());
									break;
								default:
									throw new Exception(logEvent.ToString());
							}
						}
					}
					process.WaitForExit();

					if (process.ExitCode != 0)
					{
						Log.LogError("Process exit with code: " + process.ExitCode);
						success = false;
					}
				}
				catch (Exception e)
				{
					Log.LogError(new LogEvent(e).ToString());
					success = false;
				}

				if (!success)
				{
					Log.LogError("XmlTask:" + Environment.NewLine + Serialize());
				}
				return success;
			}
		}

		public enum LogCategory
		{
			Error,
			Warning,
			Message,
		}

		public class LogEvent
		{
			public LogEvent()
			{
			}

			public LogEvent(Exception exception)
			{
				Category = LogCategory.Error;
				Message = exception == null
					? "null"
					: string.Join(Environment.NewLine,
						exception.GetType().FullName,
						string.Format("Message: {0}", exception.Message),
						string.Format(@"Source: {0}", exception.Source),
						string.Format(@"StackTrace: {0}", exception.StackTrace),
						string.Format(@"Data: {0}",
							Environment.NewLine + string.Join(Environment.NewLine,
								exception.Data.Keys.Cast<object>().Select(x =>
									string.Format("Key: {0} Value: {1}", x.ToString(), exception.Data[x].ToString())))),
						string.Format(@"InnerException: {0}", new LogEvent(exception.InnerException).Message));
			}

			public LogEvent(LogCategory category, string message)
			{
				Category = category;
				Message = message;
			}

			public LogCategory Category { get; set; }
			public string Message { get; set; }

			public override string ToString()
			{
				return string.Format("{0} {1}", Category, Message);
			}
		}
	}
}