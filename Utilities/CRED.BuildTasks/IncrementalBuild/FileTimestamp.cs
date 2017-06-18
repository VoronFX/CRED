using System;
using System.IO;
using JetBrains.Annotations;

namespace CRED.BuildTasks.IncrementalBuild
{
	public sealed class FileTimestamp
	{
		private bool Equals(FileTimestamp other)
		{
			return string.Equals(Path, other.Path) && Timestamp.Equals(other.Timestamp);
		}

		public override bool Equals([CanBeNull] object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return obj is FileTimestamp && Equals((FileTimestamp)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((Path != null ? Path.GetHashCode() : 0) * 397) ^ Timestamp.GetHashCode();
			}
		}

		public static FileTimestamp FromFile(string path)
			=> new FileTimestamp(System.IO.Path.GetFullPath(path), File.GetLastWriteTimeUtc(path));

		public bool CheckRealFileChanged()
			=> !File.Exists(Path) || File.GetLastWriteTimeUtc(Path) != Timestamp;

		public FileTimestamp(string path, DateTime timestamp)
		{
			Path = path;
			Timestamp = timestamp;
		}

		public string Path { get; }
		public DateTime Timestamp { get; }
	}
}