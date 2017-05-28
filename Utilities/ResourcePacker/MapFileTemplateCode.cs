using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourcePacker
{
	public partial class MapFileTemplate
	{
		public struct Item
		{
			public string Name { get; }
			public string Value { get; }
			public IEnumerable<string> Comment { get; }

			public Item(string name, string value, IEnumerable<string> comment)
			{
				Name = name;
				Value = value;
				Comment = comment;
			}
		}

		public string Namespace { get; }
		public IEnumerable<string> Path { get; }
	    public IEnumerable<Item> Items { get; }

	    public MapFileTemplate(string @namespace, IEnumerable<string> path, IEnumerable<Item> items)
	    {
			Namespace = @namespace;
		    Path = path;
		    Items = items;
	    }


	}
}
