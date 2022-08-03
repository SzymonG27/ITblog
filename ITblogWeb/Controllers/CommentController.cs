using Microsoft.AspNetCore.Mvc;

namespace ITblogWeb.Controllers
{
    public class CommentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
