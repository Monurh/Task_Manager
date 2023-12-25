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

                taskData.UserId = userId;
                taskData.TaskId = Guid.NewGuid();
                
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
        [HttpGet("{id:guid}")]
        public async Task<ActionResult> GetTask(Guid id)
        {
            try
            {
                _logger.LogInformation(message: "Get Task by id");
                var task = await db.Tasks.FirstOrDefaultAsync(u => u.TaskId == id);
                if (task == null)
                {
                    _logger.LogError(message: "No Task found");
                    return NotFound("No task found");
                }
                return Ok(task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpGet("Sort")]
        public async Task<ActionResult> GetSorted(SortTask sortTask)
        {
            try
            {
                _logger.LogInformation($"Executing GetSorted method with sort option: {sortTask}");

                IQueryable<Tasks> query = db.Tasks; 

                switch (sortTask)
                {
                    case SortTask.NamesAsc:
                        query = query.OrderBy(p => p.Title);
                        break;
                    case SortTask.NamesDesc:
                        query = query.OrderByDescending(p => p.Title); 
                        break;
                }

                var sortedTasks = await query.ToListAsync();

                _logger.LogInformation($"GetSorted method executed successfully with sort option: {sortTask}");
                return Ok(sortedTasks);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while processing GetSorted with sort option: {sortTask}", ex);
                return StatusCode(500, "Internal Server Error");
            }
        }
        [Authorize]
        [HttpDelete("Deleted")]
        public async Task<ActionResult> Deleted(Guid id)
        {
            try
            {
                _logger.LogInformation($"Executing DeleteTask method for task with ID: {id}");

                var task = await db.Tasks.FirstOrDefaultAsync(u => u.TaskId== id);
                if(task == null)
                {
                    return StatusCode(500, "No Task");
                }
                db.Tasks.Remove(task);
                await db.SaveChangesAsync();
                _logger.LogInformation($"DeleteTask method executed successfully for task with ID: {id}");
                return Ok(task);
            }
            catch(Exception ex)
            {
                _logger.LogError($"An error occurred while processing DeleteTask for user with ID: {id}", ex);
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
