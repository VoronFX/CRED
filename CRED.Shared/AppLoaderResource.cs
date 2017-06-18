using System;
using System.Collections.Generic;
using System.Text;

namespace CRED.Shared
{
	public sealed partial class AppLoaderResource
	{
		public AppLoaderResource(string path, ResourceType type)
		{
			Path = path;
			Type = type;
		}

		public string Path { get; }
		public ResourceType Type { get; }

		public const string ResourceTypeAttribute = "data-resource-type";

		public enum ResourceType
		{
			Script,
			Style,
			Svg
		}
	}
}
