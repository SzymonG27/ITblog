using ITblogWeb.Models.Account;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Configuration;
using ITblogWeb.Attributes;

namespace ITblogWeb.Controllers
{
    public class AccountController : Controller
    {

        Uri baseAddress = new Uri("https://localhost:7064/api/");

        [Authorize]
        public async Task<IActionResult> GetAccounts()
        {
            DataTable dt = new DataTable();
            using (var client = new HttpClient())
            {
                client.BaseAddress = baseAddress;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("AppUser");

                if (response.IsSuccessStatusCode)
                {
                    string results = await response.Content.ReadAsStringAsync();
                    dt = JsonConvert.DeserializeObject<DataTable>(results)!;
                }
                else
                {
                    Console.WriteLine("Error - status kodu: " + response.StatusCode);
                }
                ViewData.Model = dt;
            }
            return View(dt);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Login model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using(var client = new HttpClient())
            {
                client.BaseAddress = baseAddress;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var loginValues = JsonConvert.SerializeObject(model);
                var buffer = Encoding.UTF8.GetBytes(loginValues);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpResponseMessage response = await client.PostAsync("AppUser/Login", byteContent);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Pomyślnie zalogowano do serwisu!";
                    var responseString = await response.Content.ReadAsStringAsync();
                    var deserializeString = JsonConvert.DeserializeObject<Response>(responseString);
                    
                    HttpContext.Response.Cookies.Append("JwtToken", deserializeString!.Token!, new CookieOptions { HttpOnly = true });

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return View(model);
                }
            }
            
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = baseAddress;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Add token to authorization header
                var token = HttpContext.Request.Cookies["JwtToken"];
                if (token == null)
                {
                    TempData["Fail"] = "Najpierw musisz być zalogowany!";
                    return RedirectToAction("Index", "Home");
                }
                var authenticationValue = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Authorization = authenticationValue;

                //TODO
                var buffer = Encoding.UTF8.GetBytes("NOTHING");
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpResponseMessage response = await client.PostAsync("AppUser/Logout", byteContent);
                HttpContext.Response.Cookies.Delete("JwtToken");

                return RedirectToAction("Index", "Home");
            }
        }
    }
}
