using Microsoft.EntityFrameworkCore;

namespace alot.Middleware
{
    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
        {
            // Resolve the scoped AppDbContext within the request scope
            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // Get the token from the Authorization header
                var authHeader = context.Request.Headers["Authorization"].ToString();
                if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
                {
                    var token = authHeader.Substring("Bearer ".Length).Trim();

                    // Check if the token exists in the TokenValidator table
                    var tokenExists = await dbContext.validator.AnyAsync(tv => tv.Token == token);
                    if (tokenExists)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync("Token is invalid (logged out).");
                        return;
                    }
                }

                // Continue to the next middleware if the token is valid
                await _next(context);
            }
        }
    }

}
