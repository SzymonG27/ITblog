using ITblogAPI.Models;

namespace ITblogAPI.Services
{
    public interface ICommentService
    {
        Task<IEnumerable<Comment>> Get();
        Task<Comment> Get(int id);
        Task<IEnumerable<Comment>> GetFromPostId(int postId);
        Task<Comment> Create(Comment model);
        Task Update(Comment model);
        Task Delete(int id);
    }
}
