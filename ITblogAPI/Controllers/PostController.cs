using ITblogAPI.Models;
using ITblogAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ITblogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostService postService;
        public PostController(IPostService postService)
        {
            this.postService = postService;
        }

        //api/Post
        [HttpGet]
        public async Task<IEnumerable<Post>> GetPosts()
        {
            return await postService.Get();
        }

        //api/Post/1
        [HttpGet("{id}")]
        public async Task<Post> GetPosts(int id)
        {
            return await postService.Get(id);
        }

        [HttpPost]
        public async Task<ActionResult<Post>> PostPost([FromBody] Post model)
        {
            var newPost = await postService.Create(model);
            return CreatedAtAction(nameof(GetPosts), new { id = newPost.Id }, newPost);
        }

        [HttpPut]
        public async Task<ActionResult> PutPost(int id, [FromBody] Post model)
        {
            if (model.Id != id)
            {
                return BadRequest();
            }
            await postService.Update(model);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePost(int id)
        {
            var postToDelete = await postService.Get(id);
            if (postToDelete == null) return BadRequest();
            await postService.Delete(postToDelete.Id);
            return NoContent();
        }
    }
}
