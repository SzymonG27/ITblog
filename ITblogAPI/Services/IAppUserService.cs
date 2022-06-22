using ITblogAPI.Models;

namespace ITblogAPI.Services
{
    public interface IAppUserService
    {
        Task<IEnumerable<AppUser>> Get();
        Task<AppUser> Get(string id);
        Task<AppUser> CheckUser(string name, string mail);
        Task Update(AppUser model);
        Task Delete(string id);
    }
}
