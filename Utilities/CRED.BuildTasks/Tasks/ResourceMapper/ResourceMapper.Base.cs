
using System.Linq;

namespace ResourceMapper
{
	namespace Base
	{
		using System.Collections.Generic;

		public interface IResource
		{
			string Name { get; }
			IResourceDirectory ContainingDirectory { get; }
		}

		public interface IResourceDirectory : IResource
		{
			IReadOnlyDictionary<string, IResourceFile> Files { get; }
			IReadOnlyDictionary<string, IResourceDirectory> Directories { get; }
		}

		public interface IResourceFile : IResource
		{
			string Hash { get; }
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
			string IResource.Name => name;

			IResourceDirectory IResource.ContainingDirectory => parentDirectory;

			IReadOnlyDictionary<string, IResourceFile> IResourceDirectory.Files => files;

			IReadOnlyDictionary<string, IResourceDirectory> IResourceDirectory.Directories => directories;

		}

		public static class ResourceExtensionMethods
		{
			public static IEnumerable<string> GetPath(this IResource resource)
				=> resource.ContainingDirectory == null ? new[] { resource.Name } 
				: resource.ContainingDirectory.GetPath().Concat(new[] { resource.Name });

			public static IEnumerable<IResourceFile> GetFilesRecursive(this IResourceDirectory directory)
				=> directory.Files.Values.Concat(directory.Directories.Values.SelectMany(dir => dir.GetFilesRecursive()));

			public static IEnumerable<IResourceDirectory> GetDirectoriesRecursive(this IResourceDirectory directory)
				=> directory.Directories.Values.Concat(directory.Directories.Values.SelectMany(dir => dir.GetDirectoriesRecursive()));

			public static IEnumerable<IResource> GetResourcesRecursive(this IResourceDirectory directory)
				=> GetDirectoriesRecursive(directory).Concat<IResource>(GetFilesRecursive(directory));

			public static IEnumerable<IResourceFile> GetFiles(this IResourceDirectory directory)
				=> directory.Files.Values;

			public static IEnumerable<IResourceDirectory> GetDirectories(this IResourceDirectory directory)
				=> directory.Directories.Values;

			public static IEnumerable<IResource> GetResources(this IResourceDirectory directory)
				=> GetDirectories(directory).Concat<IResource>(GetFiles(directory));
		}
	}
}