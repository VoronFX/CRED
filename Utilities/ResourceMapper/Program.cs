using System;

namespace ResourceMapper
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			try
			{
				using (var stream = Console.In)
				{
					ResourceMapper.GenerateMap((ResourceMapperTask)ResourceMapperTask.Serializer().Deserialize(stream));
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
