using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly JwtService _jwtService;

        public UserController(ApplicationContext dbContext, IConfiguration configuration, ILogger<UserController> logger, AuthOptions authOptions, JwtService jwtService)
        {
            db = dbContext;
            _configuration = configuration;
            _logger = logger;
            _authOptions = authOptions;
            _jwtService = jwtService;
        }
        [HttpPost("register")]
        public async Task<ActionResult> RegisterUser([FromBody] Users userData)
        {
            try
            {
                _logger.LogInformation(message: "Metod Post Register");
                userData.UserId = Guid.NewGuid();

                // Генерация нового ключа при регистрации пользователя
                _authOptions.Key = AuthOptions.GenerateBase64Key();

                await db.Users.AddAsync(userData);
                await db.SaveChangesAsync();

                return CreatedAtAction(nameof(GetUser), new { id = userData.UserId });
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

                var token = _jwtService.GenerateJwtToken(user.UserId);

                _logger.LogInformation(message: $"User '{user.UserName}' logged in");
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error during login");
                return BadRequest(ex.Message);
            }
        }
    }
}
