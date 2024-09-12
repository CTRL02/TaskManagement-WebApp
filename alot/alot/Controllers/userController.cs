using alot.Data;
using alot.Services;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.RateLimiting;

namespace alot.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [EnableRateLimiting("otherApiPolicy")]

    public class UserController : ControllerBase
    {
        private readonly alot.Services.IAuthenticationService _authenticationService;
        private readonly AppDbContext _context;


        public UserController(alot.Services.IAuthenticationService authenticationService, AppDbContext context)
        {
            _authenticationService = authenticationService;
            _context = context;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            var response = await _authenticationService.Login(request);

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterDto request)
        {
            var response = await _authenticationService.Register(request);

            return Ok(response);
        }
     
        [AllowAnonymous]
        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            var redirectUrl = Url.Action(nameof(GoogleResponse));
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [AllowAnonymous]
        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if (!result.Succeeded)
            {
                return BadRequest(); 
            }

            
            var email = result.Principal.FindFirstValue(ClaimTypes.Email);
            var name = result.Principal.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email claim not found.");
            }

            // Call the authentication service to handle the login or registration
            var formattedName = name.Replace(" ", "");
            var token = await _authenticationService.GoogleLoginAsync(email, formattedName);

            var htmlResponse = $@"
        <html>
        <body>
            <script>
                // Send the token back to the opener (the parent window)
                window.opener.postMessage({{ token: '{token}' }}, 'http://localhost:8000');
                window.close(); // Close the popup after sending the token
            </script>
        </body>
        </html>";

            return Content(htmlResponse, "text/html");

        }
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("No token provided.");
            }

            var tokenValidator = new TokenValidator { Token = token };
            _context.validator.Add(tokenValidator);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Logged out successfully." });
        }

    }
}
