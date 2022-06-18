using ITblogAPI.Attributes;
using ITblogAPI.Infrastructure;
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
        [Authorize]
        [HttpGet]
        public async Task<IEnumerable<Post>> GetPosts([FromQuery] Pagination pagination)
        {
            if (pagination.ItemsPerPage <= 0)
            {
                pagination.ItemsPerPage = 10;
                
            }
            return await postService.Get(pagination);
        }

        //api/Post/1
        [HttpGet("{id}")]
        public async Task<Post> GetPosts(int id)
        {
            var post = await postService.Get(id);
            return post;
        }

        //api/Post/IT
        [HttpGet("Category/{category}")]
        public async Task<IEnumerable<Post>> GetPosts(string category, [FromQuery] Pagination pagination)
        {
            if (pagination.ItemsPerPage <= 0)
            {
                pagination.ItemsPerPage = 10;
            }
            var posts = await postService.Get(category, pagination);

            return posts;
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
