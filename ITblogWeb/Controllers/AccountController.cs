using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.Net.Http.Headers;

namespace ITblogWeb.Controllers
{
    public class AccountController : Controller
    {

        Uri baseAddress = new Uri("https://localhost:7064/api/");

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
            return View();
        }
    }
}
