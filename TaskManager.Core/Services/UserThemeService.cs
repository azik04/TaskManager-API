using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Dto.UserTask;
using TaskManager.Core.Dto.UserTheme;
using TaskManager.Core.Interfaces;
using TaskManager.DataProvider.Context;
using TaskManager.DataProvider.Entities;
using TeleSales.Core.Responses;

namespace TaskManager.Core.Services;

public class UserThemeService : IUserThemeService
{
    private readonly ApplicationDbContext _db;
    public UserThemeService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<BaseResponse<GetUserThemeDto>> CreateAsync(CreateUserThemeDto dto)
    {
        var userTask = await _db.UserThemes.FirstOrDefaultAsync(x => x.UserId == dto.UserId && x.ThemeId == dto.ThemeId);
        if (userTask != null)
        {
            return new BaseResponse<GetUserThemeDto>(null, false, "User is in a theme");
        }

        var data = new UserThemes
        {
            ThemeId = dto.ThemeId,
            UserId = dto.UserId,
            CreateAt = DateTime.Now,
        };
        
        await _db.UserThemes.AddAsync(data);
        await _db.SaveChangesAsync();

        var theme = await _db.Themes.FindAsync(dto.ThemeId);
        var user = await _db.Users.FindAsync(dto.UserId);

        var ndto = new GetUserThemeDto
        {
            Id = data.Id,
            UserId = data.UserId,
            UserName = user.FullName,
            ThemeId = data.ThemeId,
            ThemeName = theme.Name,
            isDeleted = data.IsDeleted,
            CreateAt = data.CreateAt,
        };

        return new BaseResponse<GetUserThemeDto>(ndto);
    }

    public async Task<BaseResponse<ICollection<GetUserThemeDto>>> GetThemeAsync(long userId)
    {
        if (userId <= 0)
            return new BaseResponse<ICollection<GetUserThemeDto>>(null);

        var datas = await _db.UserThemes
                             .Where(x => x.UserId == userId && !x.IsDeleted && !_db.Themes.Any(t => t.Id == x.ThemeId && t.CreatedBy == userId))
                             .ToListAsync();
        var dtos = new List<GetUserThemeDto>();

        foreach (var item in datas)
        {
            var theme = await _db.Themes.FindAsync(item.ThemeId);
            var user = await _db.Users.FindAsync(item.UserId);
            var crtBy = await _db.Users.FindAsync(theme.CreatedBy);

            var ndto = new GetUserThemeDto
            {
                Id = item.Id,
                UserId = item.UserId,
                UserName = user.FullName,
                ThemeId = item.ThemeId,
                ThemeName = theme.Name,
                isDeleted = item.IsDeleted,
                CreateAt = item.CreateAt,
                CreatedBy = crtBy.FullName,
            };
            dtos.Add(ndto);
        }

        return new BaseResponse<ICollection<GetUserThemeDto>>(dtos);
    }


    public async Task<BaseResponse<ICollection<GetUserThemeDto>>> GetUsersAsync(long themeId)
    {
        if (themeId <= 0)
            return new BaseResponse<ICollection<GetUserThemeDto>>(null);

        var datas = await _db.UserThemes.Where(x => x.ThemeId == themeId && !x.IsDeleted).ToListAsync();
        var dtos = new List<GetUserThemeDto>();

        foreach (var item in datas)
        {
            var theme = await _db.Themes.FindAsync(item.ThemeId);
            var user = await _db.Users.FindAsync(item.UserId);

            var ndto = new GetUserThemeDto
            {
                Id = item.Id,
                UserId = item.UserId,
                UserName = user.FullName,
                ThemeId = item.ThemeId,
                ThemeName = theme.Name,
                isDeleted = item.IsDeleted,
                CreateAt = item.CreateAt,
            };
            dtos.Add(ndto);
        }
        return new BaseResponse<ICollection<GetUserThemeDto>>(dtos);
    }

    public async Task<BaseResponse<GetUserThemeDto>> RemoveAsync(long id)
    {
        if (id <= 0)
            return new BaseResponse<GetUserThemeDto>(null);

        var data = await _db.UserThemes.SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        if (data == null)
            return new BaseResponse<GetUserThemeDto>(null);

        data.IsDeleted = true;

        _db.UserThemes.Update(data);
        await _db.SaveChangesAsync();

        var theme = await _db.Themes.FindAsync(data.ThemeId);
        var user = await _db.Users.FindAsync(data.UserId);

        var ndto = new GetUserThemeDto
        {
            Id = data.Id,
            UserId = data.UserId,
            UserName = user.FullName,
            ThemeId = data.ThemeId,
            ThemeName = theme.Name,
            isDeleted = data.IsDeleted,
            CreateAt = data.CreateAt,
        };

        return new BaseResponse<GetUserThemeDto>(ndto);
    }
}
