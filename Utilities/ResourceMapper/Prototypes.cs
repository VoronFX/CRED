using System.Collections.Generic;
using System.IO;
using ResourceMapper.Base;

namespace ResourceMapper
{
	namespace Prototypes
	{
		// Types here are just templates for generating, they should not be instantiated

		public class ResourceDirectory : ResourceDirectoryBase
		{
			//public IResourceFile DummyFile => files[nameof(DummyFile)];
			//public IResourceDirectory DummyDirectory => directories[nameof(DummyDirectory)];

			public ResourceDirectory(IResourceDirectory parentDirectory) 
				: base(nameof(ResourceDirectory), parentDirectory)
			{
				//files.Add("Dummy", new ResourceFile(nameof(DummyFile), "dummy", this));
				//directories.Add("Dummy", new ResourceDirectory(nameof(DummyDirectory), this));
			}
		}
	}
}