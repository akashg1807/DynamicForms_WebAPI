using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using DynamicForms_WebAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// ==========================================================
// 1. REGISTER SERVICES & DEPENDENCIES (Dependency Injection)
// ==========================================================

// Add controllers to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // Provides a built-in UI layout to test your endpoints

// Register our AppDbContext and hook it to the AppSettings connection string
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Fetch JWT Settings from appsettings.json
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key is missing."));

// Configure the Authentication Middleware Engine to intercept and process JWT Bearer tokens
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(secretKey),
        ClockSkew = TimeSpan.Zero // Forces immediate token expiration instead of adding a 5-minute grace period
    };
});

// Activate basic authorization infrastructure
builder.Services.AddAuthorization();

var app = builder.Build();

// ==========================================================
// 2. CONFIGURE THE HTTP REQUEST PIPELINE (Middleware Order)
// ==========================================================

// Enable Swagger UI testing page if we are running in local development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ⚠️ CRITICAL MIDDLEWARE ORDER: You must Authenticate before you can Authorize!
app.UseAuthentication(); // Intercepts requests: "Who are you? (Validates Token)"
app.UseAuthorization();  // Processes permissions: "Are you allowed to execute this action?"

app.MapControllers();

app.Run();