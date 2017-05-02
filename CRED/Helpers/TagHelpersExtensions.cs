using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace CRED.Helpers
{
	public static class TagHelpersExtensions
	{
		public static string AppendVersion(this HttpContext context, string link)
		{
			object cached;
			if (((IMemoryCache)context.RequestServices.GetService(typeof(IMemoryCache))).TryGetValue("", out cached))
				return (string)cached;
			return link;
		}
	}
}
