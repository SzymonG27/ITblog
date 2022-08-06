using ITblogWeb.Models.Comment;
using ITblogWeb.Models.Post;
using ITblogWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace ITblogWeb.Controllers
{
    public class CommentController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:7064/api/");
        private readonly ITokenService tokenService;
        public CommentController(ITokenService tokenService)
        {
            this.tokenService = tokenService;
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(PostComments model, string? returnUrl)
        {
            if (ModelState.GetFieldValidationState(model.CommentToAdd!) == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
            {
                return View(returnUrl);
            }

            using (var client = new HttpClient())
            {

                var token = HttpContext.Request.Cookies["JwtToken"];
                var handler = new JwtSecurityTokenHandler();
                var jwtSecurityToken = handler.ReadJwtToken(token);
                IEnumerable<Claim> claims = jwtSecurityToken.Claims;
                var userId = claims.FirstOrDefault(p => p.Type == "id");
                var userFirstName = claims.FirstOrDefault(p => p.Type == "FirstName");
                var userLastName = claims.FirstOrDefault(p => p.Type == "LastName");
                
                client.BaseAddress = baseAddress;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var authenticationValue = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Authorization = authenticationValue;

                Message message = new Message
                {
                    PostId = model.ResponsePost!.Id,
                    AuthorId = userId!.Value,
                    AuthorName = userFirstName!.Value,
                    AuthorSurname = userLastName!.Value,
                    Content = model.CommentToAdd!,
                    CreatedDate = DateTime.UtcNow,
                    Likes = 0
                };

                var serializedMessage = JsonConvert.SerializeObject(message);
                var buffer = Encoding.UTF8.GetBytes(serializedMessage);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = await client.PostAsync("Comment", byteContent);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Post został pomyślnie dodany";
                    return Redirect(returnUrl!);
                }
                else
                {
                    TempData["Fail"] = "Błąd! Post nie został dodany";
                    return Redirect(returnUrl!);
                }
            }
        }
    }
}
