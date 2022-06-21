using ITblogWeb.Models;
using ITblogWeb.Models.Post;
using ITblogWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace ITblogWeb.Controllers
{
    public class PostController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:7064/api/");

        private readonly ITokenService tokenService;
        public PostController(ITokenService tokenService)
        {
            this.tokenService = tokenService;
        }

        private string GetUniqueFileName(string fileName)  //(zabezpieczenie) dodaje do nazwy pliku random GUID
        {
            fileName = Path.GetFileName(fileName);
            return Path.GetFileNameWithoutExtension(fileName)
                      + "_"
                      + Guid.NewGuid().ToString().Substring(0, 9)
                      + Path.GetExtension(fileName);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var token = HttpContext.Request.Cookies["JwtToken"];
            var validateToken = tokenService.IsTokenDateValid(token!, HttpContext);
            if (validateToken == false)
            {
                TempData["Fail"] = "Twój czas logowania wygasł lub nie jesteś zalogowany! Najpierw się zaloguj";
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(PostViewModel postModel)
        {
            if (!ModelState.IsValid)
            {
                return View(postModel);
            }


            if (postModel.Photo == null)
            {
                TempData["Fail"] = "Zdjęcie musi być dodane jako wizytówka!";
                return View(postModel);
            }

            using (var client = new HttpClient())
            {
                var token = HttpContext.Request.Cookies["JwtToken"];
                var handler = new JwtSecurityTokenHandler();
                var jwtSecurityToken = handler.ReadJwtToken(token);
                IEnumerable<Claim> claims = jwtSecurityToken.Claims;
                var userId = claims.FirstOrDefault(p => p.Type == "id");

                postModel.Post!.AuthorId = userId!.Value;
                client.BaseAddress = baseAddress;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var authenticationValue = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Authorization = authenticationValue;

                var uniqueFileName = GetUniqueFileName(postModel.Photo.FileName);
                postModel.Post.ImageName = uniqueFileName;
                var finalPath = Path.Combine("wwwroot/Images", uniqueFileName);
                var stream = new FileStream(finalPath, FileMode.Create);
                postModel.Photo.CopyTo(stream);
                stream.Close();
                postModel.Photo = null;

                postModel.Post.CreatedDate = DateTime.Now;

                var postValues = JsonConvert.SerializeObject(postModel.Post);
                var buffer = Encoding.UTF8.GetBytes(postValues);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpResponseMessage response = await client.PostAsync("Post", byteContent);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Post został pomyślnie dodany";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["Fail"] = "Błąd! Post nie został dodany";
                    return View(postModel);
                }
            }
        }


        [HttpGet]
        public async Task<IActionResult> Details(HomeViewModel model)
        {
            if (model.PostId == null)
            {
                TempData["Fail"] = "Brak id posta do pokazania";
                return RedirectToAction("Index", "Home");
            }

            var token = HttpContext.Request.Cookies["JwtToken"];
            var validateToken = tokenService.IsTokenDateValid(token!, HttpContext);
            if (validateToken == false)
            {
                TempData["Fail"] = "Twój czas logowania wygasł lub nie jesteś zalogowany! Najpierw się zaloguj";
                return RedirectToAction("Index", "Home");
            }
            using (var client = new HttpClient())
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtSecurityToken = handler.ReadJwtToken(token);
                client.BaseAddress = baseAddress;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //TODO: Check if username and email are not duplicated in database

                HttpResponseMessage response = await client.GetAsync("Post/" + model.PostId);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var content = JsonConvert.DeserializeObject<ResponsePost>(responseString);
                    return View(content);
                }
                else
                {
                    TempData["Fail"] = "Błąd! Spróbuj ponownie za jakiś czas";
                    return RedirectToAction("Index", "Home");
                }

            }
        }
    }
}
