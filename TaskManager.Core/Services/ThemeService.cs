using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Dto.Themes;
using TaskManager.Core.Interfaces;
using TaskManager.DataProvider.Context;
using TaskManager.DataProvider.Entities;
using TeleSales.Core.Responses;

namespace TaskManager.Core.Services;

public class ThemeService : IThemeService
{
    private readonly ApplicationDbContext _db;
    public ThemeService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<BaseResponse<bool>> CreateAsync(CreateThemeDto theme)
    {
        var th = await _db.Users.SingleOrDefaultAsync(x => x.Id == theme.CreatedBy);
        if (th == null)
            return new BaseResponse<bool>(false , false, "User aint exists");

        var data = new Themes
        {
            Name = theme.Name,
            CreatedBy =  theme.CreatedBy ,
            CreateAt = DateTime.Now,
        };

        await _db.Themes.AddAsync(data);
        await _db.SaveChangesAsync();

        var userTheme = new UserThemes
        {
            ThemeId = data.Id,
            UserId = data.CreatedBy,
            CreateAt = DateTime.Now,
        };

        await _db.UserThemes.AddAsync(userTheme);
        await _db.SaveChangesAsync();

        var dto = new GetThemeDto
        {
            CreateAt = data.CreateAt,
            Id = data.Id,
            isDeleted = data.IsDeleted,
            CreatedBy = data.CreatedBy,
            Name = data.Name,
        };
        return new BaseResponse<bool> ( true );
    }

    public async Task<BaseResponse<ICollection<GetThemeDto>>> GetAllAsync(long userId)
    {
        if (userId <= 0)
            return new BaseResponse<ICollection<GetThemeDto>> (null);

        var data = await _db.Themes.Where(x => !x.IsDeleted && x.CreatedBy == userId).OrderByDescending(x => x.CreateAt).ToListAsync();
        if (data == null)
            return new BaseResponse<ICollection<GetThemeDto>> (null);

        var dto =  data.Select(theme => new GetThemeDto
        {
            Id = theme.Id,
            Name = theme.Name,
            CreateAt = theme.CreateAt,
            isDeleted= theme.IsDeleted,
        }).ToList();

        return new BaseResponse<ICollection<GetThemeDto>>(dto);
    }



    public async Task<BaseResponse<bool>> RemoveAsync(long id)
    {
        if (id <= 0)
            return new BaseResponse<bool>(false);

        var data = await _db.Themes.SingleOrDefaultAsync(x=> x.Id == id);
        if (data == null)
            return new BaseResponse<bool>(false);

        data.IsDeleted = true;

        _db.Themes.Update(data);
        await _db.SaveChangesAsync();

        var dto = new GetThemeDto
        {
            CreateAt = data.CreateAt,
            Id = data.Id,
            isDeleted = data.IsDeleted,
            Name = data.Name,
        };
        return new BaseResponse<bool>(true);
    }

    public async Task<BaseResponse<bool>> UpdateAsync(long id, UpdateThemeDto theme)
    {
        if (id <= 0)
            return new BaseResponse<bool>(null);

        var data = await _db.Themes.SingleOrDefaultAsync(x => x.Id == id);
        if (data == null)
            return new BaseResponse<bool>(null);

        data.Name = theme.Name;

        _db.Themes.Update(data);
        await _db.SaveChangesAsync();

        var dto = new GetThemeDto
        {
            CreateAt = data.CreateAt,
            Id = data.Id,
            isDeleted = data.IsDeleted,
            Name = data.Name,
        };
        return new BaseResponse<bool>(true);
    }
}
