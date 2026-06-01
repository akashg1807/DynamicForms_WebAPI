using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using DynamicForms_WebAPI.Data;
using DynamicForms_WebAPI.Models;

namespace DynamicForms_WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // 1. Look up the user in the database by their username
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username);

            // 2. Validate credentials (checking plain-text strings for assignment scope simplicity)
            if (user == null || user.PasswordHash != request.Password)
            {
                return Unauthorized(new { message = "Invalid username or password." });
            }

            // 3. Prepare the Token Signature Key using the secret from appsettings.json
            var jwtSettings = _config.GetSection("Jwt");
            var secretKey = jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Secret Key is missing.");
            var key = Encoding.UTF8.GetBytes(secretKey);

            // 4. Define the User Identity Claims (Crucial for RBAC!)
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.RoleId.ToString()) 
            };

            // 5. Generate the Security Token Descriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(2), // Token remains valid for 2 hours
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            // 6. Serialize the Token into a readable Bearer String
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // 7. Return the response to React
            return Ok(new AuthResponse
            {
                Token = tokenString,
                Username = user.Username,
                RoleId = user.RoleId
            });
        }
    }
}