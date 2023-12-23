using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task_Manager.DB;
using Task_Manager.Model;

namespace Task_Manager.Controllers
{
    [ApiController]
    [Route("api/task")]
    public class TaskController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly ApplicationContext db;
        private readonly IConfiguration _configuration;

        public TaskController(ApplicationContext dbContext, IConfiguration configuration, ILogger<UserController> logger)
        {
            db = dbContext;
            _configuration = configuration;
            _logger = logger;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("addtask")]
        public async Task<ActionResult> AddTask([FromBody] Tasks taskData)
        {
            try
            {
                var userIdClaim = HttpContext.User.FindFirst("UserId");

                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
                {
                    return BadRequest("Invalid or missing UserId");
                }

                // Предполагается, что taskData имеет поле UserId для привязки к задаче
                taskData.UserId = userId;
                taskData.TaskId = Guid.NewGuid();

                // Вставка новой задачи с UserId, извлеченным из токена
                db.Tasks.Add(taskData);
                await db.SaveChangesAsync();

                return CreatedAtAction(nameof(GetTask), new { id = taskData.TaskId });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpGet("id:guid")]
        public async Task<ActionResult> GetTask(Guid id)
        {
            try
            {
                _logger.LogInformation(message: "Get Task id");
                var task = await db.Tasks.FirstOrDefaultAsync(u => u.TaskId == id);
                if(task==null)
                {
                    _logger.LogError(message: "Not Task");
                    return NotFound("Not task");
                }
                return Ok(task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }

}
