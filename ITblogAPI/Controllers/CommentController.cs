using ITblogAPI.Models;
using ITblogAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ITblogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService commentService;
        public CommentController(ICommentService commentService)
        {
            this.commentService = commentService;
        }

        //api/Comment
        [HttpGet]
        public async Task<IEnumerable<Comment>> GetComments()
        {
            return await commentService.Get();
        }

        //api/Comment/1
        [HttpGet("{id}")]
        public async Task<Comment> GetComments(int id)
        {
            return await commentService.Get(id);
        }

        //api/Comment/1/FromPostId
        [HttpGet("{postId}/FromPostId")]
        public async Task<IEnumerable<Comment>> GetCommentsFromPostId(int postId)
        {
            var post = await commentService.GetFromPostId(postId);
            if (post == null)
            {
                return Enumerable.Empty<Comment>();
            }
            return post!;
        }

        //api/Comment
        [HttpPost]
        public async Task<ActionResult<Comment>> PostComment([FromBody] Comment model)
        {
            var newComment = await commentService.Create(model);
            return CreatedAtAction(nameof(GetComments), new { id = newComment.Id }, newComment);
        }

        //api/Comment
        [HttpPut]
        public async Task<ActionResult> PutComment([FromBody] Comment model)
        {
            await commentService.Update(model);
            return NoContent();
        }

        //api/Comment/1
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteComment(int id)
        {
            var commentToDelete = await commentService.Get(id);
            if (commentToDelete == null) return BadRequest();
            await commentService.Delete(commentToDelete.Id);
            return NoContent();
        }
    }
}
