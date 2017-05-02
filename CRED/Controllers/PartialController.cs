using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace CRED.Controllers
{
    [SwaggerIgnore]
    public class PartialController : Controller
    {
	    public IActionResult Component(string component) => PartialView($"~/Views/Components/{component}.cshtml");

		public IActionResult AboutComponent() => PartialView();

        public IActionResult AppComponent() => PartialView();

        public IActionResult ContactComponent() => PartialView();

        public IActionResult IndexComponent() => PartialView();

        public IActionResult LoginComponent() => PartialView();

        public IActionResult RegisterComponent() => PartialView();

        public IActionResult FileUploadComponent() => PartialView();
    }
}
