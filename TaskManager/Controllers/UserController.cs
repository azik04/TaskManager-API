using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Core.Dto.Users;
using TaskManager.Core.Interfaces;

namespace TaskManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;
        public UserController(IUserService service)
        {
            _service = service;
        }

        [Authorize(Policy = "User")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id , UpdateUserDto dto)
        {
            var res = await _service.Update(id, dto);
            if (res.Success)
                return Ok(res);

            return BadRequest();
        }


        [Authorize(Policy = "User")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var res = await _service.GetById(id);
            if (res.Success)
                return Ok(res);

            return BadRequest();
        }
    }
}
