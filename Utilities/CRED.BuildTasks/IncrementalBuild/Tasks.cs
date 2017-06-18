using System;
using System.Runtime.Serialization;
using Microsoft.Build.Framework;
using Newtonsoft.Json;

namespace CRED.BuildTasks.IncrementalBuild
{
	public sealed class CheckCache : TaskBase
	{
		[Required]
		[ExpandPath]
		[DataMember]
		public string[] InputFiles { get; set; }

		[DataMember]
		public string[] Parameters { get; set; }

		[Output]
		[DataMember]
		public bool NeedBuild { get; set; }

		[Output]
		[DataMember]
		public string Cache { get; set; }

		protected override bool ExecuteWork()
		{
			NeedBuild = IncrementalBuild.Cache.CheckNeedBuild(IncrementalBuildCacheFile, InputFiles, new { Parameters }, out Cache cache);
			Cache = JsonConvert.SerializeObject(cache);

			return true;
		}
	}
	
	public sealed class SaveCache : TaskBase
	{

		[Required]
		[DataMember]
		public string Cache { get; set; }

		[Required]
		[ExpandPath]
		[DataMember]
		public string[] OutputFiles { get; set; }

		protected override bool ExecuteWork()
		{
			JsonConvert.DeserializeObject<Cache>(Cache)
				.SaveBuildCache(IncrementalBuildCacheFile, OutputFiles);
			return true;
		}
	}
}