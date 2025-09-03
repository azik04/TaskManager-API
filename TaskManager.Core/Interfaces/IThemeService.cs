using TaskManager.Core.Dto.Themes;
using TeleSales.Core.Responses;

namespace TaskManager.Core.Interfaces;

public interface IThemeService
{
    Task<BaseResponse<bool>> CreateAsync(CreateThemeDto theme);
    Task<BaseResponse<ICollection<GetThemeDto>>> GetAllAsync(long userId);
    Task<BaseResponse<bool>> RemoveAsync(long id);
    Task<BaseResponse<bool>> UpdateAsync(long id , UpdateThemeDto theme);


}
