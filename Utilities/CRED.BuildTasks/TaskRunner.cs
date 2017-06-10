using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using CRED.BuildTasks.Wrapper;

namespace CRED.BuildTasks
{
	public class TaskRunner
	{
		private static void Main(string[] args)
		{
			using (Console.Out)
			using (var logger = new Logger(Console.Out))
			{
				try
				{
					var tasks = Assembly.GetExecutingAssembly()
						.GetTypes()
						.Where(x => x.BaseType == typeof(TaskWrapperBase))
						.Select(x => new
						{
							Serializer = TaskWrapperBase.ExplicitOnlySerializer(x),
							TaskWrapperType = x,
							TaskType = Assembly
								.GetExecutingAssembly()
								.GetTypes()
								.First(x2 => x2.BaseType == typeof(Task)
											 && x2.GetConstructors().Any(c =>
												 c.GetParameters().Select(p => p.ParameterType).SequenceEqual(new[] { x })
											 )
								)
						})
						.ToArray();

					using (var stream = Console.In)
					using (var reader = XmlReader.Create(stream))
					{
						reader.ReadStartElement(TaskWrapperBase.TasksNodeName);

						while (true)
						{
							var matchedTask = tasks.FirstOrDefault(x => x.Serializer.CanDeserialize(reader));
							if (matchedTask == null) break;
							var task = (Task)Activator.CreateInstance(matchedTask.TaskType, matchedTask.Serializer.Deserialize(reader));
							task.Logger = logger;
							task.Execute();
						}
					}
				}
				catch (Exception e)
				{
					logger.Log(e);
				}
			}
		}

		public abstract class Task
		{
			public Logger Logger { get; set; }

			public abstract void Execute();
		}
	}

	public sealed class Logger : IDisposable
	{
		public Logger(TextWriter textWriter)
		{
			XmlWriter = XmlWriter.Create(textWriter);
			XmlWriter.WriteStartDocument();
			XmlWriter.WriteStartElement(TaskWrapperBase.LogNodeName);
		}

		private XmlWriter XmlWriter { get; }

		private XmlSerializer Serializer { get; }
			= new XmlSerializer(typeof(LogEvent));

		private object LogLock { get; } = new object();

		public void Log(LogCategory category, string message)
		{
			Log(new LogEvent(category, message));
		}

		public void Log(Exception exception)
		{
			Log(new LogEvent(exception));
		}

		public void Log(LogEvent logEvent)
		{
			lock (LogLock)
			{
				Serializer.Serialize(XmlWriter, logEvent);
				XmlWriter.Flush();
			}
		}

		private bool Disposed { get; set; }

		public void Dispose()
		{
			if (!Disposed)
			{
				Disposed = true;
				XmlWriter.WriteEndElement();
				XmlWriter.WriteEndDocument();
			}
			XmlWriter.Dispose();
		}
	}
}