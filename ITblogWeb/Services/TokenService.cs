using System.IdentityModel.Tokens.Jwt;

namespace ITblogWeb.Services
{
    public class TokenService : ITokenService
    {

        public bool IsTokenDateValid(string token, HttpContext context) //True - valid | False - invalid
        {
            if (token == null)
            {
                return false;
            }
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);
            if (jwtSecurityToken.ValidTo <= DateTime.UtcNow)
            {
                context.Response.Cookies.Delete("JwtToken");
                return false;
            }
            return true;
        }
    }
}
