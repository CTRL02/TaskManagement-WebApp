using Microsoft.AspNetCore.Identity.Data;
using alot.Data;
using System.IdentityModel.Tokens.Jwt;

namespace alot.Services
{
    public interface IAuthenticationService
    {
        Task<IResult> Register(RegisterDto request);
        Task<IResult> Login(LoginDto request);
        Task<IResult> GoogleLoginAsync(string email, string name);

    }
}
