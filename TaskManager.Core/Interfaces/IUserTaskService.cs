using TaskManager.Core.Dto.UserTask;
using TaskManager.Core.Dto.UserTheme;
using TeleSales.Core.Responses;

namespace TaskManager.Core.Interfaces
{
    public interface IUserTaskService
    {
        Task<BaseResponse<GetUserTaskDto>> CreateAsync(CreateUserTaskDto dto);
        Task<BaseResponse<ICollection<GetUserTaskDto>>> GetUsersAsync(long themeId);
        Task<BaseResponse<ICollection<GetUserTaskDto>>> GetTaskAsync(long userId);
        Task<BaseResponse<GetUserTaskDto>> RemoveAsync(long id);
    }
}
