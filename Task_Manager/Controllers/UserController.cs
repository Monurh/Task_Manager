using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public UserController(ApplicationContext dbContext, IConfiguration configuration , ILogger<UserController> logger)
        {
            db = dbContext;
            _configuration = configuration;
            _logger = logger;
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
                if(user ==null)
                {
                    _logger.LogError(message:"Not User");
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
    }
}
