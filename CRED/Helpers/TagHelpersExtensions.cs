using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers.Internal;
using Microsoft.Extensions.Caching.Memory;

namespace CRED.Helpers
{
	public static class TagHelpersExtensions
	{
		//public LinkTagHelper(
		//	IHostingEnvironment hostingEnvironment,
		//	IMemoryCache cache,
		//	HtmlEncoder htmlEncoder,
		//	JavaScriptEncoder javaScriptEncoder,
		//	IUrlHelperFactory urlHelperFactory)
		//	: base(urlHelperFactory, htmlEncoder)
		//{
		//	HostingEnvironment = hostingEnvironment;
		//	Cache = cache;
		//	JavaScriptEncoder = javaScriptEncoder;
		//}

		private static FileVersionProvider _fileVersionProvider;

		public static string AppendVersion(this HttpContext context, string link)
		{
			//if (_fileVersionProvider == null)
			//{
			//	_fileVersionProvider = new FileVersionProvider(
			//		HostingEnvironment.WebRootFileProvider,
			//		Cache,
			//		ViewContext.HttpContext.Request.PathBase);
			//}
			object cached;
			if (((IMemoryCache)context.RequestServices.GetService(typeof(IMemoryCache))).TryGetValue(link, out cached))
				return (string)cached;
			return link;
		}
	}
}
