using Task_Manager.DB;
using Task_Manager.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace Task_Manager.Controllers
{
    [ApiController]
    [Route("api/pagination")]
    public class PaginationController : ControllerBase
    {
        private readonly ILogger<PaginationController> _logger;
        private readonly ApplicationContext db;
        private readonly PaginationParameters _paginationParameters;

        public PaginationController(ApplicationContext dbContext, ILogger<PaginationController> logger)
        {
            db = dbContext;
            _logger = logger;
            _paginationParameters = new PaginationParameters();
        }

        [HttpGet("Pagination")]
        public async Task<IActionResult> GetItems([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                _paginationParameters.PageNumber = pageNumber;
                _paginationParameters.PageSize = pageSize;

                _logger.LogInformation($"Executing GetItems method for page {pageNumber} with page size {pageSize}");

                var items = await db.Tasks
                    .Skip((_paginationParameters.PageNumber - 1) * _paginationParameters.PageSize)
                    .Take(_paginationParameters.PageSize)
                    .ToListAsync();

                if (items.Count == 0)
                {
                    _logger.LogWarning("No items found during pagination.");
                     return StatusCode(500,"Paginated items, No items found");
                }

                _logger.LogInformation($"GetItems method executed successfully for page {pageNumber} with page size {pageSize}");
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while processing GetItems", ex);
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }
    }
}
