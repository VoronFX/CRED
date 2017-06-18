using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bridge;
using Bridge.Html5;
using CRED.Client;

namespace CRED.Client
{
	[FileName(nameof(Loader))]
	public class Loader
	{
		//[Convention(Notation.LowerCamelCase)]
		//public class ProgressEventAgrs
		//{
		//	private bool LengthComputable { get; }

		//	private long IsLoadComplete { get; }

		//	private long Total { get; }

		//	private double Progress => (double)IsLoadComplete / Total;

		//}

		private class Resource
		{
			public Resource(string url, Action<string> loadEnd)
			{
				Url = url;
				LoadEnd = loadEnd;
			}

			public XMLHttpRequest Request { get; set; }
			public string Url { get; }
			public double Progress { get; set; }
			public bool IsLoadComplete { get; set; }
			public Action<string> LoadEnd { get; }
		}

		private IEnumerator<Resource> NextToInit { get; set; }
		private Resource[] Resources { get; set; }
		private TaskCompletionSource<bool> LoadComplete { get; }
			= new TaskCompletionSource<bool>();
		public double TotalProgress { get; private set; }

		public Loader(IEnumerable<KeyValuePair<string, Action<string>>> resources)
		{
			Resources = resources.Select(res => new Resource(res.Key, res.Value)).ToArray();
			foreach (var resource in Resources)
			{
				resource.Request = new XMLHttpRequest()
				{
					OnProgress = args =>
					{
						var progress = args;
						if (progress.LengthComputable)
						{
							resource.Progress = (double)progress.Loaded / progress.Total;
							UpdateProgress();
						}
					},
					OnLoadEnd = args =>
					{
						try
						{
							if (resource.Request.Status != 200)
								throw new Exception(
									$"Error loading resources. Resource {resource.Url} failed with status code {resource.Request.Status} ({resource.Request.StatusText}).");

							resource.Progress = 1;
							UpdateProgress();
							resource.IsLoadComplete = true;
							if (resource == NextToInit.Current)
							{
								do
								{
									NextToInit.Current.LoadEnd?.Invoke(NextToInit.Current.Request.ResponseText);
									if (NextToInit.Current == Resources.Last())
										LoadComplete.SetResult(true);
								} while (NextToInit.MoveNext() && NextToInit.Current.IsLoadComplete);
							}
						}
						catch (Exception e)
						{
							if (LoadComplete.TrySetException(e))
								Array.ForEach(Resources, x => x.Request.Abort());
						}
					}
				};
				resource.Request.Open("GET", resource.Url);
			}
			NextToInit = Resources.ToEnumerator<Resource>();
			NextToInit.MoveNext();
		}

		public async Task Load()
		{
			Array.ForEach(Resources, x => x.Request.Send());
			await LoadComplete.Task;
		}

		private void UpdateProgress()
		{
			TotalProgress = Resources.Select(x => x.Progress).Sum() / Resources.Length;
			Progress?.Invoke(TotalProgress);
		}

		public Action<double> Progress { get; set; }
	}

}

namespace CRED.Shared
{
	
	[FileName(nameof(Loader))]
	public sealed partial class AppLoaderResource
	{
	}
}