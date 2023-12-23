using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Task_Manager.Authentication;
using Task_Manager.DB;
using Task_Manager.Model;

namespace Task_Manager.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly ApplicationContext db;
        private readonly IConfiguration _configuration;
        private readonly AuthOptions _authOptions;

        public UserController(ApplicationContext dbContext, IConfiguration configuration, ILogger<UserController> logger, AuthOptions authOptions)
        {
            db = dbContext;
            _configuration = configuration;
            _logger = logger;
            _authOptions = authOptions;
        }

        [HttpPost("register")]
        public async Task<ActionResult> RegisterUser([FromBody] Users userData)
        {
            try
            {
                _logger.LogInformation(message: "Metod Post Register");
                userData.UserId = Guid.NewGuid();

                await db.Users.AddAsync(userData);
                await db.SaveChangesAsync();

                var token = GenerateJwtToken(userData.UserId.ToString(), userData.Rolle, userData.UserId.ToString());
                return CreatedAtAction(nameof(GetUser), new { id = userData.UserId }, new { Token = token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("id:guid")]
        public async Task<ActionResult> GetUser(Guid id)
        {
            try
            {
                _logger.LogInformation(message: "Get User id");
                var user = await db.Users.FirstOrDefaultAsync(u => u.UserId == id);
                if (user == null)
                {
                    _logger.LogError(message: "Not User");
                    return NotFound("Not user");
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginModel loginModel)
        {
            try
            {
                _logger.LogInformation(message: "Login method called");
                var user = await db.Users.FirstOrDefaultAsync(u => u.UserName == loginModel.UserName);

                if (user == null || user.Password != loginModel.Password)
                {
                    return BadRequest("Invalid login or password");
                }

                var token = GenerateJwtToken(user.UserId.ToString(), user.Rolle, user.UserId.ToString());

                _logger.LogInformation(message: $"User '{user.UserName}' logged in");
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error during login");
                return BadRequest(ex.Message);
            }
        }

        private string GenerateJwtToken(string id, string role, string userId)
        {
            var authOptions = _configuration.GetSection("AuthOptions").Get<AuthOptions>();
            var securityKey = authOptions.GetSymmetricSecurityKey();
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, id),
                new Claim(ClaimTypes.Role, role),
                new Claim("UserId", userId) // Добавление утверждения UserId в токен
            };

            var token = new JwtSecurityToken(
                issuer: authOptions.Issuer,
                audience: authOptions.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(authOptions.TokenLifetime),
                signingCredentials: credentials
            );

            var tokenHandler = new JwtSecurityTokenHandler();

            return tokenHandler.WriteToken(token);
        }
    }
}
