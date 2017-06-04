using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

namespace AzureResourcesExtractor
{
	[Serializable]
	public class AzureResourcesExtractorTask : Task
	{
		[Required]
		[XmlElement]
		public string SourcesDirectory { get; set; }
		
		[XmlElement]
		public string Namespace { get; set; }

		[Required]
		[XmlElement]
		public string OutputDirectory { get; set; }

		[Required]
		[XmlElement]
		public string OutputMapsDirectory { get; set; }

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
