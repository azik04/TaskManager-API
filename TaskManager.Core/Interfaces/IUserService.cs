using TaskManager.Core.Dto.Users;
using TaskManager.DataProvider.Enums;
using TeleSales.Core.Responses;

namespace TaskManager.Core.Interfaces;

public interface IUserService
{
    Task<BaseResponse<bool>> Create(CreateUserDto user);
    Task<BaseResponse<ICollection<GetUserDto>>> GetAdmin();
    Task<BaseResponse<ICollection<GetUserDto>>> GetUser();
    Task<BaseResponse<GetUserDto>> GetById(long id);

    Task<BaseResponse<bool>> Remove(long id);
    Task<BaseResponse<bool>> ChangeRole(long id, Role role);
    Task<BaseResponse<bool>> Update(long id , UpdateUserDto user);
}
