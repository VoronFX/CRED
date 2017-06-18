using System;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace CRED.BuildTasks
{
	//public class PrepareBuildTasks : Task
	//{
	//	[Required]
	//	public string SourceDir { get; set; }

	//	[Required]
	//	public string CopyTargetDir { get; set; }

	//	[Output]
	//	public string LatestCopyDir { get; set; }


	//	private static void CloneDirectory(string root, string dest)
	//	{
	//		if (!Directory.Exists(dest))
	//		{
	//			Directory.CreateDirectory(dest);
	//		}

	//		foreach (var directory in Directory.GetDirectories(root))
	//		{
	//			string dirName = Path.GetFileName(directory);
	//			if (!Directory.Exists(Path.Combine(dest, dirName)))
	//			{
	//				Directory.CreateDirectory(Path.Combine(dest, dirName));
	//			}
	//			CloneDirectory(directory, Path.Combine(dest, dirName));
	//		}

	//		foreach (var file in Directory.GetFiles(root))
	//		{
	//			File.Copy(file, Path.Combine(dest, Path.GetFileName(file)));
	//		}
	//	}

	//	public override bool Execute()
	//	{
	//		var files = Directory.GetFiles(SourceDir, "*", SearchOption.TopDirectoryOnly);
	//		var version = files.Max(x => File.GetLastWriteTime(x)).Ticks.ToString();
	//		LatestCopyDir = Path.Combine(CopyTargetDir, version);

	//		if (!Directory.Exists(LatestCopyDir))
	//		{
	//			Directory.CreateDirectory(LatestCopyDir);
	//			foreach (var file in files)
	//			{
	//				File.Copy(file, Path.Combine(LatestCopyDir, Path.GetFileName(file)));
	//			}
	//			File.WriteAllText(Path.Combine(CopyTargetDir, "LatestVersion.txt"), version);
	//			//CloneDirectory(SourceDir, LatestCopyDir);
	//		}

	//		foreach (var directory in Directory.GetDirectories(CopyTargetDir)
	//			.Where(x => x != LatestCopyDir))
	//		{
	//			try
	//			{
	//				Directory.Delete(directory, true);
	//			}
	//			catch (Exception e)
	//			{
	//				Log.LogWarningFromException(e);
	//			}
	//		}

	//		return true;
	//	}
	//}

	public class PrepareVersionCopy : Task
	{
		[Required]
		public string[] SourceFiles { get; set; }

		[Required]
		public string VersionFile { get; set; }

		[Output]
		public string LatestVersion { get; set; }

		public override bool Execute()
		{
			LatestVersion = SourceFiles.Max(x => File.GetLastWriteTime(x)).Ticks.ToString();
			File.WriteAllText(VersionFile, LatestVersion);
			return true;
		}
	}
}