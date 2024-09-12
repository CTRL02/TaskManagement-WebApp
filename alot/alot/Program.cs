using Microsoft.AspNetCore.Authentication.JwtBearer;
using alot.Data;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using alot;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using alot.Services;
using Swashbuckle.AspNetCore.Filters;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Google;
using alot.Middleware;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
var builder = WebApplication.CreateBuilder(args);

var myOptions = new MyRateLimitOptions();
var otherApiRateLimitOptions = new MyRateLimitOptions();
builder.Configuration.GetSection("OtherApiRateLimit").Bind(otherApiRateLimitOptions);
var otherApiPolicy = "otherApiPolicy";
builder.Services.AddRateLimiter(options =>
{
    options.AddSlidingWindowLimiter("sliding", rateLimiterOptions =>
    {
        rateLimiterOptions.PermitLimit = 100; 
        rateLimiterOptions.Window = TimeSpan.FromSeconds(60);
        rateLimiterOptions.SegmentsPerWindow = 4;
        rateLimiterOptions.QueueLimit = 5;
    });

    options.AddFixedWindowLimiter(otherApiPolicy, rateLimiterOptions =>
    {
        rateLimiterOptions.PermitLimit = otherApiRateLimitOptions.PermitLimit;
        rateLimiterOptions.Window = TimeSpan.FromSeconds(otherApiRateLimitOptions.Window); 
        rateLimiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        rateLimiterOptions.QueueLimit = otherApiRateLimitOptions.QueueLimit;  
    });
});
//builder.Logging.ClearProviders();
//builder.Logging.AddConsole();
//builder.Logging.AddDebug();

//// Set the minimum log level to Debug
//builder.Services.Configure<LoggerFilterOptions>(options =>
//{
//    options.MinLevel = LogLevel.Debug;
//});

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

//swagger with bearer
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

//add dbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//add identity
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();


var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>();

builder.Services.AddSingleton(jwtOptions);

//Adding jwt authentication 
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters

    {
        ValidateIssuer = true,
        ValidateLifetime = true,
        ValidIssuer = jwtOptions.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtOptions.Audience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey))
    };


}).AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"] ;
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
});

builder.Services.AddAuthorization();



var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRateLimiter();
app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseMiddleware<TokenValidationMiddleware>();
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();


app.Run();

