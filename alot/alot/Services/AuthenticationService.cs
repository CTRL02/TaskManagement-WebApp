using alot.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace alot.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<User> _userManager;
        private readonly JwtOptions _jwtOptions;

        public AuthenticationService(UserManager<User> userManager, JwtOptions jwtOptions)
        {
            _userManager = userManager;
            _jwtOptions = jwtOptions;
        }

        public async Task<IResult> Register(RegisterDto  request)
        {
            var userByEmail = await _userManager.FindByEmailAsync(request.Email);
            var userByUsername = await _userManager.FindByNameAsync(request.UserName);
            if (userByEmail is not null || userByUsername is not null)
            {
                return Results.BadRequest($"User with email {request.Email} or username {request.UserName} already exists.");
            }

            User user = new()
            {
                Email = request.Email,
                UserName = request.UserName,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                return Results.BadRequest($"Unable to register user {request.UserName} errors: {GetErrorsText(result.Errors)}");
            }

            var loginResult = await Login(new LoginDto { Email = request.Email, Password = request.Password });
            return loginResult;
        }

        public async Task<IResult> Login(LoginDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
            {
                return Results.BadRequest($"Unable to authenticate user");
            }


            var authClaims = new List<Claim>
            {
              new(ClaimTypes.Name, user.UserName),                
              new(ClaimTypes.Email, user.Email),                  
              new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), 
              new(ClaimTypes.NameIdentifier, user.Id)
            };


            var token = GetToken(authClaims);

            return Results.Ok(token);
        }
        public async Task<IResult> GoogleLoginAsync(string email, string name)
        {
            // Check if user exists by email
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // Create user if not exists
                user = new User
                {
                    Email = email,
                    UserName = name,
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    return Results.BadRequest($"Unable to register user {name} from Google Sign-In: {GetErrorsText(result.Errors)}");
                }
            }

            // Generate token
            var authClaims = new List<Claim>
            {
                new(ClaimTypes.Name, user.UserName),
                new(ClaimTypes.Email, user.Email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(ClaimTypes.NameIdentifier, user.Id)
            };

            return Results.Ok(GetToken(authClaims));
        }
        private string GetToken(IEnumerable<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SigningKey));
            var handler = new JwtSecurityTokenHandler();
            var tokenDesc = new SecurityTokenDescriptor
            {
                Issuer = _jwtOptions.Issuer,
                Audience =  _jwtOptions.Audience,
                Subject = new ClaimsIdentity(authClaims),
                SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                };
            var token = handler.CreateToken(tokenDesc);
            var accessToken = handler.WriteToken(token);
            return accessToken;
        }

        private string GetErrorsText(IEnumerable<IdentityError> errors)
        {
            return string.Join(", ", errors.Select(error => error.Description).ToArray());
        }
    }
}
