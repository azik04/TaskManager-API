using TaskManager.Core.Dto.Comment;
using TeleSales.Core.Responses;

namespace TaskManager.Core.Interfaces;

public interface ICommentService
{
    Task<BaseResponse<bool>> Create(CreateCommentDto comment);
    Task<BaseResponse<ICollection<GetCommentDto>>> GetByTask(long taskId);
    Task<BaseResponse<bool>> Remove(long id);
    Task<BaseResponse<bool>> Update(long id , UpdateCommentDto comment);
}

