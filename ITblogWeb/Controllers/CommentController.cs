using ITblogWeb.Models;
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
                
                var token = HttpContext.Request.Cookies["JwtToken"];    //Get claims from token (check ITBlogAPI>Services>TokenService
                var handler = new JwtSecurityTokenHandler();            //To get all claims)
                var jwtSecurityToken = handler.ReadJwtToken(token);
                IEnumerable<Claim> claims = jwtSecurityToken.Claims;
                var userId = claims.FirstOrDefault(p => p.Type == "id");
                var userFirstName = claims.FirstOrDefault(p => p.Type == "FirstName");
                var userLastName = claims.FirstOrDefault(p => p.Type == "LastName");
                
                client.BaseAddress = baseAddress;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var authenticationValue = new AuthenticationHeaderValue("Bearer", token);   //Add Bearer authorization
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

        [HttpPost]
        public async Task<IActionResult> Like(PostComments model, string? returnUrl)
        {
            using (var client = new HttpClient())
            {

                var token = HttpContext.Request.Cookies["JwtToken"];   //Read token claims
                var handler = new JwtSecurityTokenHandler();
                var jwtSecurityToken = handler.ReadJwtToken(token);
                IEnumerable<Claim> claims = jwtSecurityToken.Claims;
                var userId = claims.FirstOrDefault(p => p.Type == "id");

                client.BaseAddress = baseAddress;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var authenticationValue = new AuthenticationHeaderValue("Bearer", token);      //Add Bearer authorization
                client.DefaultRequestHeaders.Authorization = authenticationValue;

                var getComment = await client.GetAsync("Comment/" + model.MessageId);

                if (getComment.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    TempData["Fail"] = "Nie ma takiego komentarza!";
                    return Redirect(returnUrl!);
                }

                if (getComment.IsSuccessStatusCode)
                {
                    var responseString = await getComment.Content!.ReadAsStringAsync();
                    var content = JsonConvert.DeserializeObject<Message>(responseString);

                    var checkStatus = await client.GetAsync("CommentRelation/CheckRelation/" + userId!.Value + "&" + model.MessageId);
                    if (checkStatus.StatusCode == System.Net.HttpStatusCode.NotFound) //NotFound - AddRelation, OK-Relation exists
                    {
                        CommentLikesRelation relation = new CommentLikesRelation
                        {
                            UserId = userId!.Value,
                            CommentId = model.MessageId
                        };

                        var serializedRelation = JsonConvert.SerializeObject(relation);
                        var buffer = Encoding.UTF8.GetBytes(serializedRelation);
                        var byteContent = new ByteArrayContent(buffer);
                        byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                        var postRelation = await client.PostAsync("CommentRelation", byteContent);
                        if (postRelation.IsSuccessStatusCode)
                        {
                            content!.Likes += 1;
                            var serializedMessage = JsonConvert.SerializeObject(content);
                            var bufferM = Encoding.UTF8.GetBytes(serializedMessage);
                            var byteContentM = new ByteArrayContent(bufferM);
                            byteContentM.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                            var addLike = await client.PutAsync("Comment", byteContentM);
                            if (addLike.IsSuccessStatusCode)
                            {
                                TempData["Success"] = "Like został dodany pomyślnie!";
                                return Redirect(returnUrl!);
                            }
                            TempData["Fail"] = "Wystąpił problem! Spróbuj ponownie za jakiś czas.";
                            return Redirect(returnUrl!);
                        }
                        TempData["Fail"] = "Wystąpił problem! Spróbuj ponownie za jakiś czas.";
                        return Redirect(returnUrl!);
                    }
                    else if (checkStatus.StatusCode == System.Net.HttpStatusCode.OK) //OK - DeleteRelation, NotFound - Relation not exists
                    {
                        var removeRelation = await client.DeleteAsync("CommentRelation/" + userId!.Value + "&" + model.MessageId);
                        if (removeRelation.IsSuccessStatusCode)
                        {
                            content!.Likes -= 1;
                            var serializedMessage = JsonConvert.SerializeObject(content);
                            var bufferM = Encoding.UTF8.GetBytes(serializedMessage);
                            var byteContentM = new ByteArrayContent(bufferM);
                            byteContentM.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                            var removeLike = await client.PutAsync("Comment", byteContentM);
                            if (removeLike.IsSuccessStatusCode)
                            {
                                TempData["Success"] = "Like został usunięty pomyślnie!";
                                return Redirect(returnUrl!);
                            }
                            TempData["Fail"] = "Wystąpił problem! Spróbuj ponownie za jakiś czas.";
                            return Redirect(returnUrl!);
                        }
                        TempData["Fail"] = "Wystąpił problem! Spróbuj ponownie za jakiś czas.";
                        return Redirect(returnUrl!);
                    }
                    TempData["Fail"] = "Wystąpił problem! Spróbuj ponownie za jakiś czas."; //Another status code (not predictive)
                    return Redirect(returnUrl!);
                }
                TempData["Fail"] = "Wystąpił problem! Spróbuj ponownie za jakiś czas.";
                return Redirect(returnUrl!);

            }
        }
    }
}
