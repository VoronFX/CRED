using System;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using NSwag.Annotations;

namespace CRED.Controllers
{
	[SwaggerIgnore]
	public sealed class LongPollingFileWatcher : Controller
	{
		public IHostingEnvironment HostingEnvironment { get; }

		public LongPollingFileWatcher(IHostingEnvironment hostingEnvironment)
		{
			HostingEnvironment = hostingEnvironment;
		}

		public async Task<IActionResult> Watch(string path)
		{
			var fileChanges = new TaskCompletionSource<bool>();
			HostingEnvironment.WebRootFileProvider.Watch(path)
				.RegisterChangeCallback(o => fileChanges.SetResult(true), null);

			while (!HttpContext.RequestAborted.IsCancellationRequested)
			{
				if (await Task.WhenAny(fileChanges.Task, Task.Delay(10000)) == fileChanges.Task
					&& fileChanges.Task.IsCompleted && fileChanges.Task.Result)
					return Ok();
			}
			return BadRequest();
		}
	}
}
