using ITblogAPI.Data;
using ITblogAPI.Models;
using ITblogAPI.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ITblogAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppUserController : ControllerBase
    {
        private readonly IAppUserService userService;
        private readonly UserManager<AppUser> userManager;
        private readonly IConfiguration configuration;
        public AppUserController(IAppUserService _userService, UserManager<AppUser> _userManager, 
            IConfiguration _configuration)
        {
            userService = _userService;
            userManager = _userManager;
            configuration = _configuration;
        }


        //api/AppUser
        [HttpGet]
        public async Task<IEnumerable<AppUser>> GetUsers()
        {
            return await userService.Get();
        }


        //api/AppUser/1
        [HttpGet("{id}")]
        public async Task<AppUser> GetUsers(string id)
        {
            return await userService.Get(id);
        }


        //api/AppUser/Register
        [HttpPost]
        [Route("Register")]
        public async Task<ActionResult<AppUser>> RegisterUser([FromBody] Register model)
        {
            if (!ModelState.IsValid || model == null)
            {
                return new UnauthorizedObjectResult(new { Message = "Rejestracja zakończona niepowodzeniem" });
            }
            var identityUser = new AppUser() { UserName = model.UserName, Email = model.Email, FirstName = model.FirstName, 
                LastName = model.LastName};
            var result = await userManager.CreateAsync(identityUser, model.Password);

            if (!result.Succeeded)
            {
                var dictionary = new ModelStateDictionary();
                foreach (IdentityError error in result.Errors)
                {
                    dictionary.AddModelError(error.Code, error.Description);
                }

                return new UnauthorizedObjectResult(new { Message = "Rejestracja zakończona niepowodzeniem", 
                    Errors = dictionary});
            }
            return Ok(new { Message = "Rejestracja zakończona sukcesem" });
        }


        //api/AppUser/Login
        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult<AppUser>> LoginUser([FromBody] Login model)
        {
            AppUser identityUser = await ValidateUser(model);
            if (!ModelState.IsValid || model == null || identityUser == null)
            {
                return new UnauthorizedObjectResult(new { Message = "Logowanie zakończone niepowodzeniem" });
            }
            var token = GenerateToken(identityUser);

            //If you need to use webApi on client side change HttpOnly value to false -- 
            // It will end up protecting against XSS attacks
            HttpContext.Response.Cookies.Append("JwtToken", token, new CookieOptions { HttpOnly = true });


            return Ok(new { Token=token, Message="Logowanie zakończone sukcesem" });

        }


        [HttpPost]
        [Route("Logout")]
        public ActionResult LogoutUser()
        {

            HttpContext.Response.Cookies.Delete("JwtToken");
            //TODO: Delete JWT token (not only cookie)

            return Ok();
        }


        [HttpPut]
        public async Task<ActionResult> PutUser(string id, [FromBody] AppUser model) //change password etc.
        {
            if (model.Id != id)
            {
                return BadRequest();
            }
            await userService.Update(model);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(string id)
        {
            var userToDelete = await userService.Get(id);
            if (userToDelete == null) return BadRequest();
            await userService.Delete(userToDelete.Id);
            return NoContent();
        }



        //Essentials

        private async Task<AppUser> ValidateUser(Login model) //Todo: Change location to AppUserService
        {
            var identityUser = await userManager.FindByNameAsync(model.LoginUser);
            if (identityUser != null)
            {
                var result = userManager.PasswordHasher.VerifyHashedPassword(identityUser, identityUser.PasswordHash, model.Password);

                if (result == PasswordVerificationResult.Success)
                {
                    return identityUser;
                }
                return null!;
            }

            return null!;
        }

        private string GenerateToken(AppUser identityUser)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtSettings = configuration.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings.GetSection("Key").Value);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, identityUser.UserName.ToString()),
                    new Claim(ClaimTypes.Email, identityUser.Email),
                    new Claim("FirstName", identityUser.FirstName!)
                }),
                Expires = DateTime.UtcNow.AddSeconds(jwtSettings.GetValue<double>("ExpiryTimeInSeconds")),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Audience = jwtSettings.GetSection("Audience").Value,
                Issuer = jwtSettings.GetSection("Issuer").Value
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }
    }
}
