using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Dto.UserTask;
using TaskManager.Core.Interfaces;
using TaskManager.DataProvider.Context;
using TaskManager.DataProvider.Entities;
using TeleSales.Core.Responses;
 
namespace TaskManager.Core.Services;

public class UserTaskService : IUserTaskService
{
    private readonly ApplicationDbContext _db;
    public UserTaskService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<BaseResponse<bool>> CreateAsync(CreateUserTaskDto dto)
    {
        var userTask = await _db.UserTask.SingleOrDefaultAsync(x => x.UserId == dto.UserId && x.TaskId == dto.TaskId);
        if (userTask != null)
        {
            userTask.IsDeleted = false;
            userTask.hasUpdate = false;
            
            _db.UserTask.Update(userTask);
            await _db.SaveChangesAsync();
        }

        if (userTask == null)
        {
            var data = new UserTasks
            {
                TaskId = dto.TaskId,
                UserId = dto.UserId,
                CreateAt = DateTime.Now,
            };

            await _db.UserTask.AddAsync(data);
            await _db.SaveChangesAsync();
        }


        return new BaseResponse<bool>(true);
    }

    public async Task<BaseResponse<ICollection<GetUserTaskDto>>> GetTaskAsync(long userId)
    {
        if (userId <= 0)
            return new BaseResponse<ICollection<GetUserTaskDto>>(null);

        var datas = await _db.UserTask.Where(x => x.UserId == userId && !x.IsDeleted).ToListAsync();
        var dtos = new List<GetUserTaskDto>();

        foreach (var item in datas)
        {
            var theme = await _db.Tasks.FindAsync(item.TaskId);
            var user = await _db.Users.FindAsync(item.UserId);

            var ndto = new GetUserTaskDto
            {
                Id = item.Id,
                UserId = item.UserId,
                UserName = user.FullName,
                TaskId = item.TaskId,
                TaskName = theme.TaskName,
                isDeleted = item.IsDeleted,
                CreateAt = item.CreateAt,
                hasUpdate = item.hasUpdate
            };
            dtos.Add(ndto);
        }
        return new BaseResponse<ICollection<GetUserTaskDto>>(dtos);
    }

    public async Task<BaseResponse<ICollection<GetUserTaskDto>>> GetUsersAsync(long taskId)
    {
        if (taskId <= 0)
            return new BaseResponse<ICollection<GetUserTaskDto>>(null);

        var datas = await _db.UserTask.Where(x => x.TaskId == taskId && !x.IsDeleted).ToListAsync();
        var dtos = new List<GetUserTaskDto>();

        foreach (var item in datas)
        {
            var theme = await _db.Tasks.FindAsync(item.TaskId);
            var user = await _db.Users.FindAsync(item.UserId);

            var ndto = new GetUserTaskDto
            {
                Id = item.Id,
                UserId = item.UserId,
                UserName = user.FullName,
                TaskId = item.TaskId,
                TaskName = theme.TaskName,
                isDeleted = item.IsDeleted,
                CreateAt = item.CreateAt,
                hasUpdate = item.hasUpdate
            };
            dtos.Add(ndto);
        }
        return new BaseResponse<ICollection<GetUserTaskDto>>(dtos);
    }

    public async Task<BaseResponse<bool>> RemoveAsync(long id)
    {
        if (id <= 0)
            return new BaseResponse<bool>(false);

        var data = await _db.UserTask.SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        if (data == null)
            return new BaseResponse<bool>(false);

        data.IsDeleted = true;

        var theme = await _db.Tasks.FindAsync(data.TaskId);
        var user = await _db.Users.FindAsync(data.UserId);

        var ndto = new GetUserTaskDto
        {
            Id = data.Id,
            UserId = data.UserId,
            UserName = user.FullName,
            TaskId = data.TaskId,
            TaskName = theme.TaskName,
            isDeleted = data.IsDeleted,
            CreateAt = data.CreateAt,
            hasUpdate = data.hasUpdate
        };

        return new BaseResponse<bool>(true);
    }
}
