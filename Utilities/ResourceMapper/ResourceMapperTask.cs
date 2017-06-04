using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

namespace ResourceMapper
{
	[Serializable]
	public class ResourceMapperTask : Task
	{
		[XmlElement]
		public string Namespace { get; set; }

		[XmlElement]
		public string TopClassName { get; set; }

		[Required]
		[XmlElement]
		public string[] InputFiles { get; set; }

		[XmlElement]
		public string OutputFile { get; set; }

		[Required]
		[XmlElement]
		public string RootDirectory { get; set; }

		[Required]
		[XmlElement]
		public string TaskAssemblyPath { get; set; }

		public static XmlSerializer Serializer()
		{
			var overrides = new XmlAttributeOverrides();
			var attributes = new XmlAttributes { XmlIgnore = true };
			foreach (var prop in typeof(ResourceMapperTask)
				.GetProperties()
				.Where(x => !x.GetCustomAttributes(typeof(XmlElementAttribute))
				.Any()))
			{
				overrides.Add(prop.DeclaringType, prop.Name, attributes);
			}
			return new XmlSerializer(typeof(ResourceMapperTask), overrides);
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
						FileName = Path.Combine(TaskAssemblyPath, "ResourceMapper.exe"),
					}
				};
				process.Start();
				using (var stream = process.StandardInput)
				{
					Serializer().Serialize(stream, this);
				}
				var result = process.StandardOutput.ReadToEnd();
				process.WaitForExit();

				if (!string.IsNullOrWhiteSpace(result))
					Log.LogError(result);
				else if (process.ExitCode != 0)
					Log.LogError("Process exit with code: " + process.ExitCode);
				else
					return true;
			}
			catch (Exception e)
			{
				Log.LogErrorFromException(e);
			}
			return false;
		}
	}
}
