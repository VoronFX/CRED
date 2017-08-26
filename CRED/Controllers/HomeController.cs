using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using NSwag.Annotations;

namespace CRED.Controllers
{
	[SwaggerIgnore]
	public class HomeController : Controller
	{
		//public IActionResult Index()
		//{
		//    ViewData["Title"] = "Home";
		//    return View();
		//}

		public IActionResult Error()
		{
			return View();
		}

		private readonly IFileProvider _fileProvider;

		public HomeController(IFileProvider fileProvider)
		{
			_fileProvider = fileProvider;
		}

		public IActionResult Index()
		{
			var contents = _fileProvider.GetDirectoryContents(ControllerContext.HttpContext.Request.GetUri().LocalPath);
			var uri = ControllerContext.HttpContext.Request.GetUri();
			return View(contents);
		}
	}
}
