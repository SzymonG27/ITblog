using ITblogAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ITblogAPI.Services
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<AppUser> userManager;
        private readonly IConfiguration configuration;
        public TokenService (UserManager<AppUser> userManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.configuration = configuration;
        }

        public async Task<AppUser> ValidateUser(Login model)
        {
            var identityUser = await userManager.FindByNameAsync(model.LoginUser);
            if (identityUser != null)
            {
                var result = userManager.PasswordHasher.VerifyHashedPassword(identityUser, identityUser.PasswordHash, model.Password);

                if (result == PasswordVerificationResult.Success)
                {
                    return identityUser;
                }
                return null!;
            }

            return null!;
        }

        public string GenerateToken(AppUser identityUser)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtSettings = configuration.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings.GetSection("Key").Value);
            var timeExpiration = DateTime.UtcNow.AddSeconds(jwtSettings.GetValue<double>("ExpiryTimeInSeconds"));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("id", identityUser.Id),
                    new Claim(ClaimTypes.Name, identityUser.UserName.ToString()),
                    new Claim(ClaimTypes.Email, identityUser.Email),
                    new Claim("FirstName", identityUser.FirstName!),
                    new Claim("LastName", identityUser.LastName!)
                }),
                Expires = DateTime.UtcNow.AddSeconds(jwtSettings.GetValue<double>("ExpiryTimeInSeconds")),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Audience = jwtSettings.GetSection("Audience").Value,
                Issuer = jwtSettings.GetSection("Issuer").Value
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }

        public bool ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtSettings = configuration.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings.GetSection("Key").Value);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
