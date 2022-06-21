namespace ITblogWeb.Services
{
    public interface ITokenService
    {
        bool IsTokenDateValid(string token, HttpContext context);
    }
}
