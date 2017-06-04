﻿  
  
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

namespace AzureResourcesExtractor
{
	public partial class AzureResourcesExtractor
	{
		private static Action<AzureResourcesExtractorTask> Execute { get; set; }

		private static void Main(string[] args)
		{
			using (xmlWriter = XmlWriter.Create(Console.Out))
			{
				xmlWriter.WriteStartDocument();
				xmlWriter.WriteStartElement("Log");
				try
				{
					using (var stream = Console.In)
					{
						Execute((AzureResourcesExtractorTask)AzureResourcesExtractorTask.Serializer().Deserialize(stream));
					}
				}
				catch (Exception e)
				{
					Log(e);
				}
				xmlWriter.WriteEndElement();
				xmlWriter.WriteEndDocument();
			}
		}

		private static void Log(LogCategory category, string message)
		{
			Log(new LogEvent(category, message));
		}

		private static void Log(Exception exception)
		{
			Log(new LogEvent(exception));
		}

		private static readonly object LogLock = new object();

		private static XmlWriter xmlWriter;

		private static void Log(LogEvent logEvent)
		{
			lock (LogLock)
			{
				new XmlSerializer(typeof(LogEvent)).Serialize(xmlWriter, logEvent);
				xmlWriter.Flush();
			}
		}
	}

	public enum LogCategory
	{
		Error,
		Warning,
		Message,
	}

	[Serializable]
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

	[Serializable]
	public class AzureResourcesExtractorTask : Task
	{
		[Required]
		[XmlElement]
		public System.String TaskAssemblyPath { get; set; }
	
		[Required]
		[XmlElement]
		public System.String SourcesDirectory { get; set; }
	
		[Required]
		[XmlElement]
		public System.String OutputDirectory { get; set; }
	
		[Required]
		[XmlElement]
		public System.String OutputMapsDirectory { get; set; }
	
		[Required]
		[XmlElement]
		public System.String RootDirectory { get; set; }
	
		[XmlElement]
		public System.String Namespace { get; set; }
	
        public static XmlSerializer Serializer()
		{
			var overrides = new XmlAttributeOverrides();
			var attributes = new XmlAttributes { XmlIgnore = true };
			foreach (var prop in typeof(AzureResourcesExtractorTask)
				.GetProperties()
				.Where(x => !x.GetCustomAttributes(typeof(XmlElementAttribute))
				.Any()))
			{
				overrides.Add(prop.DeclaringType, prop.Name, attributes);
			}
			return new XmlSerializer(typeof(AzureResourcesExtractorTask), overrides);
		}

		public override bool Execute()
		{
			try
			{
				var process = new Process
				{
					StartInfo =
					{
						CreateNoWindow = true,
						UseShellExecute = false,
						RedirectStandardOutput = true,
						RedirectStandardInput = true,
						FileName = Path.Combine(TaskAssemblyPath, "AzureResourcesExtractor.exe"),
					}
				};
				process.Start();
				var success = true;
				using (var stream = process.StandardInput)
				{
					Serializer().Serialize(stream, this);
				}
				using (var reader = XmlReader.Create(process.StandardOutput))
				{
					reader.ReadStartElement("Log");
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
				return success;
			}
			catch (Exception e)
			{
				Log.LogError(new LogEvent(e).ToString());
			}
			return false;
		}
	}
}
 