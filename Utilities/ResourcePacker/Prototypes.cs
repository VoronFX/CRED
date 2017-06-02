using System.Collections.Generic;

namespace ResourcePacker
{
	namespace Prototypes
	{
		// Types here are just templates for generating, they should not be instantiated

		public interface IResourceDirectory
		{
			IReadOnlyDictionary<string, IResourceFile> Files { get; }
			IReadOnlyDictionary<string, IResourceDirectory> Directories { get; }
			string Name { get; }
			IResourceDirectory ParentDirectory { get; }
		}

		public interface IResourceFile
		{
			string Name { get; }
			string Hash { get; }
			IResourceDirectory ContainingDirectory { get; }
		}

		internal sealed class ResourceFile : IResourceFile
		{
			public ResourceFile(string name, string hash, IResourceDirectory containingDirectory)
			{
				Name = name;
				Hash = hash;
				ContainingDirectory = containingDirectory;
			}

			public string Name { get; }
			public string Hash { get; }
			public IResourceDirectory ContainingDirectory { get; }
		}

		public abstract class ResourceDirectoryBase : IResourceDirectory
		{
			protected readonly Dictionary<string, IResourceFile> files
				= new Dictionary<string, IResourceFile>();
			protected readonly Dictionary<string, IResourceDirectory> directories
				= new Dictionary<string, IResourceDirectory>();
			protected readonly IResourceDirectory parentDirectory;
			protected readonly string name;

			protected ResourceDirectoryBase(string name, IResourceDirectory parentDirectory)
			{
				this.parentDirectory = parentDirectory;
				this.name = name;
			}

			IReadOnlyDictionary<string, IResourceFile> IResourceDirectory.Files => files;

			IReadOnlyDictionary<string, IResourceDirectory> IResourceDirectory.Directories => directories;

			string IResourceDirectory.Name => name;

			IResourceDirectory IResourceDirectory.ParentDirectory => parentDirectory;
		}

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