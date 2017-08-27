using System;
using System.IO;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace CRED.BuildTasks.IncrementalBuild
{
	public sealed class FileStamp
	{
		public bool CheckRealFileChanged()
		{
			if (!File.Exists(Path))
				return true;
			var fileInfo = new FileInfo(Path);
			if (fileInfo.LastWriteTimeUtc == LastWriteTimeUtc)
				return false;
			if (fileInfo.Length != Length)
				return true;
			return FileUtilities.GetHashForFile(Path) != Hash;
		}

		[CanBeNull]
		public FileStamp NewStampIfRealFileChanged()
		{
			if (!File.Exists(Path))
				return new FileStamp(Path);
			var fileInfo = new FileInfo(Path);
			if (fileInfo.LastWriteTimeUtc == LastWriteTimeUtc)
				return null;
			if (fileInfo.Length != Length)
				return new FileStamp(Path, fileInfo.LastWriteTimeUtc, fileInfo.Length, FileUtilities.GetHashForFile(Path));
			var hash = FileUtilities.GetHashForFile(Path);
			if (hash != Hash)
				return new FileStamp(Path, fileInfo.LastWriteTimeUtc, fileInfo.Length, hash);
			return null;
		}

		public FileStamp(string path)
		{
			Path = path;
			var fileInfo = new FileInfo(path);
			LastWriteTimeUtc = fileInfo.LastWriteTimeUtc;
			Length = fileInfo.Length;
			Hash = FileUtilities.GetHashForFile(path);
		}

		[JsonConstructor]
		public FileStamp(string path, DateTime lastWriteTimeUtc, long length, string hash)
		{
			Path = path;
			LastWriteTimeUtc = lastWriteTimeUtc;
			Length = length;
			Hash = hash;
		}

		public string Path { get; }
		public DateTime LastWriteTimeUtc { get; }
		public long Length { get; }
		public string Hash { get; }
	}
}