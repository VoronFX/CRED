using System;

namespace AzureResourcesExtractor
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			try
			{
				using (var stream = Console.In)
				{
					Extractor.ExtractAzureResources((AzureResourcesExtractorTask)AzureResourcesExtractorTask.Serializer().Deserialize(stream));
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}
	}
}
