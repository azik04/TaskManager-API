using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Core.Dto.Tasks;
using TaskManager.Core.Interfaces;

namespace TaskManager.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TaskController : ControllerBase
{
    private readonly ITaskService _service;
    public TaskController(ITaskService service)
    {
        _service = service;
    }


    [HttpPost]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> Create(CreateTaskDto task, long userId)
    {
        var res = await _service.Create(task, userId);
        if (res.Success)
            return Ok(res);

        return BadRequest(res);
    }


    [HttpGet()]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> GetAll(long userId, int statusId)
    {
        var res = await _service.GetAllAsync(userId, statusId);
        if (res.Success)
            return Ok(res);

        return BadRequest(res);
    }


    [HttpGet("{id}")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> GetById(long id, long userId)
    {
        var res = await _service.GetById(id, userId);
        if (res.Success)
            return Ok(res);

        return BadRequest(res);
    }


    [HttpDelete("{id}")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> Remove(long id)
    {
        var res = await _service.Remove(id);
        if (res.Success)
            return Ok(res);

        return BadRequest(res);
    }


    [HttpPut("{id}")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> Update(long id, UpdateTaskDto task)
    {
        var res = await _service.Update(id, task);
        if (res.Success)
            return Ok(res);

        return BadRequest(res);
    }


    [HttpPut("{id}/Complite")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> Complite(long id)
    {
        var res = await _service.Complite(id);
        if (res.Success)
            return Ok(res);

        return BadRequest(res);
    }
}
