using ITblogAPI.Models;

namespace ITblogAPI.Services
{
    public interface ICommentLikesRelationService
    {
        Task<IEnumerable<CommentLikesRelation>> Get();
        Task<CommentLikesRelation> Get(int id);
        Task<CommentLikesRelation> GetRecord(string userId, int commentId);
        Task<CommentLikesRelation> Create(CommentLikesRelation model);
        Task Delete(int rowId);
        Task<bool> IsRelation(CommentLikesRelation model);
    }
}
