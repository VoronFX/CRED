using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pso2Reparser
{
	class Program
	{
		static void Main(string[] args)
		{
			var parsed = File.ReadAllLines(@"D:\Downloads\enmeinpach250417.csv")
				.Where(s => !string.IsNullOrWhiteSpace(s))
				.Select(s =>
			{
				int qb = s.IndexOf('"');
				int qe = s.LastIndexOf('"');

				var splitted = s.Split(new[] { ", " }, StringSplitOptions.None);

				return new []{splitted[0], splitted[1], splitted[2], splitted[3], s.Substring(qb, qe - qb) };
			}).ToArray();

			List<string> ne = new List<string>();
			ne.AddRange(parsed.Select(s => s[0]).Distinct());
			ne.AddRange(parsed.Select(s => s[1]).Distinct());
			ne.AddRange(parsed.Select(s => s[2]).Distinct());
			ne.AddRange(parsed.Select(s => s[3]).Distinct());
			//ne.AddRange(parsed.Select(s => s[4]));

			File.WriteAllText("distinct.txt", string.Join(", ", ne));

			for (int i = 0; i < 5; i++)
			{
				Console.WriteLine(parsed.Select(s => s[i]).Distinct().Count());
			}
			Console.ReadKey();
		}
	}
}
