using ITblogAPI.Models;
using ITblogAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace ITblogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentRelationController : ControllerBase
    {
        private readonly ICommentLikesRelationService service;
        public CommentRelationController(ICommentLikesRelationService service)
        {
            this.service = service;
        }

        //api/CommentRelation
        [HttpGet]
        public async Task<IEnumerable<CommentLikesRelation>> GetCommentRelations()
        {
            return await service.Get();
        }

        //api/CommentRelation/1
        [HttpGet("{id}")]
        public async Task<CommentLikesRelation> GetCommentRelations(int id)
        {
            return await service.Get(id);
        }

        //api/CommentRelation/xxx-xxx-xxx&1
        [HttpGet("CheckRelation/{userId}&{commentId}")]
        public async Task<ActionResult> GetCommentRelations(string userId, int commentId) //Check relation for AddLike (ItBlogWeb CommentController)
        {
            bool check = await service.IsRelation(new CommentLikesRelation
            {
                UserId = userId,
                CommentId = commentId
            });
            if (check == true)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        //api/CommentRelation
        [HttpPost]
        public async Task<ActionResult<CommentLikesRelation>> PostCommentRelation([FromBody] CommentLikesRelation model)
        {
            var newCommentRelation = await service.Create(model);
            return CreatedAtAction(nameof(GetCommentRelations), new { id = newCommentRelation.Id }, newCommentRelation);
        }

        //api/CommentRelation/xxx-xxx-xxx&1
        [HttpDelete("{userId}&{commentId}")]
        public async Task<ActionResult> DeleteCommentRelation(string userId, int commentId)
        {
            var releationToDelete = await service.GetRecord(userId, commentId);
            if (releationToDelete == null) return BadRequest();
            await service.Delete(releationToDelete.Id);
            return NoContent();
        }
    }
}
