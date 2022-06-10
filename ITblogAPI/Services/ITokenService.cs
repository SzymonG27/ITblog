using ITblogAPI.Models;

namespace ITblogAPI.Services
{
    public interface ITokenService
    {
        Task<AppUser> ValidateUser(Login model);
        string GenerateToken(AppUser identityUser);
        bool ValidateToken(string token);
    }
}
