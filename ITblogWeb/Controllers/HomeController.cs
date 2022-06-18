using ITblogWeb.Models;
using ITblogWeb.Models.Post;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace ITblogWeb.Controllers
{
    public class HomeController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:7064/api/");

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var token = HttpContext.Request.Cookies["JwtToken"];
            if (token == null)
            {
                return View();
            }
            using(var client = new HttpClient())
            {
                client.BaseAddress = baseAddress;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var authenticationValue = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Authorization = authenticationValue;
                

                HttpResponseMessage response = await client.GetAsync("Post?Page=1&ItemsPerPage=50");
                var responseString = await response.Content.ReadAsStringAsync();
                var deserializeResponse = JsonConvert.DeserializeObject<IEnumerable<ResponsePost>>(responseString);
                return View(deserializeResponse);
            }
            
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}