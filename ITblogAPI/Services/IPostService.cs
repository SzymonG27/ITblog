using ITblogAPI.Models;

namespace ITblogAPI.Services
{
    public interface IPostService
    {
        Task<IEnumerable<Post>> Get();
        Task<Post> Get(int id);
        Task<Post> Create(Post post);
        Task Update(Post post);
        Task Delete(int id);
    }
}
