using ITblogAPI.Infrastructure;
using ITblogAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace ITblogAPI.Services
{
    public interface IPostService
    {
        Task<IEnumerable<Post>> Get([FromQuery] Pagination pagination);
        Task<Post> Get(int id);
        Task<IEnumerable<Post>> Get(string category, [FromQuery] Pagination pagination);
        Task<Post> Create(Post post);
        Task Update(Post post);
        Task Delete(int id);
    }
}
