using ITblogAPI.Data;
using ITblogAPI.Models;
using ITblogAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace ITblogAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppUserController : ControllerBase
    {
        private readonly IAppUserService userService;
        public AppUserController(IAppUserService _userService)
        {
            userService = _userService;
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

        [HttpPost]
        public async Task<ActionResult<AppUser>> PostUser([FromBody] AppUser model)
        {
            var newUser = await userService.Create(model);
            return CreatedAtAction(nameof(GetUsers), new { id = newUser.Id }, newUser);
        }

        [HttpPut]
        public async Task<ActionResult> PutUser(string id, [FromBody] AppUser model)
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
