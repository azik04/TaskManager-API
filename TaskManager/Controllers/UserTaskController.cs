using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Core.Dto.UserTask;
using TaskManager.Core.Interfaces;

namespace TaskManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserTaskController : ControllerBase
    {
        private readonly IUserTaskService _service;

        public UserTaskController(IUserTaskService service)
        {
            _service = service;
        }


        [HttpPost]
        [Authorize(Policy = "User")]
        public async Task<IActionResult> AddUserToTheme(CreateUserTaskDto vm)
        {
            var result = await _service.CreateAsync(vm);
            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }


        [HttpDelete("{id}")]
        [Authorize(Policy = "User")]
        public async Task<IActionResult> RemoveUserFromTask(long id)
        {
            var result = await _service.RemoveAsync(id);
            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }


        [HttpGet("Task/{taskId}/Users")]
        [Authorize(Policy = "User")]
        public async Task<IActionResult> GetUsersByTaskId(long taskId)
        {
            var result = await _service.GetUsersAsync(taskId);
            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }


        [HttpGet("User/{userId}/Task")]
        [Authorize(Policy = "User")]
        public async Task<IActionResult> GetTaskByUserId(long userId)
        {
            var result = await _service.GetTaskAsync(userId);
            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }
    }
}
