
namespace ResourceMapper
{
	namespace Base
	{
		using System.Collections.Generic;

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
			internal ResourceFile(string name, string hash, IResourceDirectory containingDirectory)
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
			protected internal readonly Dictionary<string, IResourceFile> files
				= new Dictionary<string, IResourceFile>();
			protected internal readonly Dictionary<string, IResourceDirectory> directories
				= new Dictionary<string, IResourceDirectory>();
			protected internal readonly IResourceDirectory parentDirectory;
			protected internal readonly string name;

			protected internal ResourceDirectoryBase(string name, IResourceDirectory parentDirectory)
			{
				this.parentDirectory = parentDirectory;
				this.name = name;
			}
			string IResourceDirectory.Name => name;

			IResourceDirectory IResourceDirectory.ParentDirectory => parentDirectory;

			IReadOnlyDictionary<string, IResourceFile> IResourceDirectory.Files => files;

			IReadOnlyDictionary<string, IResourceDirectory> IResourceDirectory.Directories => directories;

		}

		public static class ResourceExtensionMethods
		{
			public static string GetPath(this IResourceDirectory directory, string directorySeparatorChar) =>
				directory.ParentDirectory?.GetPath(directorySeparatorChar) + directorySeparatorChar + directory.Name;
		}
	}
}