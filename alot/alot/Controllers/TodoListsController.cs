using System.Security.Claims;
using alot.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

namespace alot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [EnableRateLimiting("sliding")]

    public class TodoListsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TodoListsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/TodoLists
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoList>>> GetTodoLists(int pageNumber = 1)
        {
            var pageSize = 10; 
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var totalCount = await _context.TodoLists.CountAsync(t => t.UserId == userId);
            var todoLists = await _context.TodoLists
                .Where(t => t.UserId == userId)
                .OrderBy(t => t.DateTime)  
                .Skip((pageNumber - 1) * pageSize)  
                .Take(pageSize)  // Take the current page size (fixed to 10)
                .Select(t => new { t.Id, t.Todo, t.DateTime, t.IsCompleted })
                .ToListAsync();

            // Create pagination metadata
            var paginationMetadata = new
            {
                totalCount,
                pageSize,
                currentPage = pageNumber,
                totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            // Return the paginated results with metadata
            return Ok(new { paginationMetadata, todoLists });
        }


        // PUT: api/TodoLists/update/5
        [HttpPut("updateStatus/{id}")]
        public async Task<IActionResult> PutTodoList(int id, TodoCompleteDTO todoList)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var affected = await _context.TodoLists
                .Where(model => model.Id == id && model.UserId == userId)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(m => m.IsCompleted, todoList.IsCompleted)
                );

            if (affected == 0)
            {
                return NotFound();
            }

            return Ok();
        }

        // PUT: api/TodoLists/update/5/List
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateTodoList(int id, TodoUpdate todoList)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var affected = await _context.TodoLists
                .Where(model => model.Id == id && model.UserId == userId)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(m => m.Todo, todoList.Todo)
                    .SetProperty(m => m.DateTime, todoList.DateTime)
                );

            if (affected == 0)
            {
                return NotFound();
            }

            return Ok();
        }

        // POST: api/TodoLists
        [HttpPost("add")]
        public async Task<ActionResult<TodoList>> PostTodoList(TodoUpdate todoList)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEntity = await _context.Users.FindAsync(userId);

            var dto = new TodoList
            {
                DateTime = todoList.DateTime,
                IsCompleted = false,
                Todo = todoList.Todo,
                UserId = userId,
            };

            _context.TodoLists.Add(dto);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTodoLists), new { id = dto.Id }, new { dto.Id, dto.Todo, dto.DateTime, dto.IsCompleted });
        }

        // DELETE: api/TodoLists/Delete/5
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteTodoList(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var affected = await _context.TodoLists
                .Where(model => model.Id == id && model.UserId == userId)
                .ExecuteDeleteAsync();

            if (affected == 0)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}
