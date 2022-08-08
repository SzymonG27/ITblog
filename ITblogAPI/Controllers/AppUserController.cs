using ITblogAPI.Attributes;
using ITblogAPI.Data;
using ITblogAPI.Models;
using ITblogAPI.Services;
using Microsoft.AspNetCore.Authentication;
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
        private readonly ITokenService tokenService;
        public AppUserController(IAppUserService _userService, UserManager<AppUser> _userManager, 
                ITokenService tokenService)
        {
            userService = _userService;
            userManager = _userManager;
            this.tokenService = tokenService;
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

        //api/AppUser/Check
        [HttpGet]
        [Route("Check")]
        public async Task<AppUser> GetUser(string name, string mail)
        {
            return await userService.CheckUser(name, mail);
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
        public async Task<ActionResult<AppUser>> LoginUser(Login model)
        {
            AppUser identityUser = await tokenService.ValidateUser(model);
            if (!ModelState.IsValid || model == null || identityUser == null)
            {
                return new UnauthorizedObjectResult(new { Message = "Logowanie zakończone niepowodzeniem" });
            }
            var token = tokenService.GenerateToken(identityUser);

            var validate = tokenService.ValidateToken(token);

            if (validate == true)
            {
                return Ok(new { Token = token, Message = "Logowanie zakończone sukcesem" });
            }
            else
            {
                return BadRequest();
            }

        }


        [HttpPost]
        [Authorize]
        [Route("Logout")]
        public ActionResult LogoutUser()
        {

            //HttpContext.Response.Cookies.Delete("JwtToken");
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
    }
}
