using Microsoft.AspNetCore.Mvc;

namespace ITblogWeb.Controllers
{
    public class PostController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
