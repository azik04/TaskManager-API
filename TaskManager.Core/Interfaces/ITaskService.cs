using TaskManager.Core.Dto.Tasks;
using TeleSales.Core.Responses;

namespace TaskManager.Core.Interfaces;

public interface ITaskService
{
    Task<BaseResponse<bool>> Create(CreateTaskDto task, long userId);
    Task<BaseResponse<ICollection<GetTaskDto>>> GetAllAsync(long themeId, int statusId);
    Task<BaseResponse<GetTaskDto>> GetById(long id, long userId);
    Task<BaseResponse<bool>> Remove(long id);
    Task<BaseResponse<bool>> Update(long id, UpdateTaskDto task);
    Task<BaseResponse<bool>> Complite(long id);
}
