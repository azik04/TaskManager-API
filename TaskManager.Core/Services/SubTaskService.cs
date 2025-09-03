using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Dto.SubTask;
using TaskManager.Core.Interfaces;
using TaskManager.DataProvider.Context;
using TaskManager.DataProvider.Entities;
using TeleSales.Core.Responses;

namespace TaskManager.Core.Services;

public class SubTaskService : ISubTaskService
{
    private readonly ApplicationDbContext _db;
    public SubTaskService(ApplicationDbContext db)
    {
        _db = db; 
    } 
    public async Task<BaseResponse<bool>> Create(CreateSubTaskDto subTask)
    {
        var data = new SubTasks
        {
            DeadLine = subTask.DeadLine,
            Name = subTask.Name,
            Priority = subTask.Priority,
            UserId = subTask.UserId,
            CreateAt = DateTime.Now,
            TaskId = subTask.TaskId,

        };
        
        await _db.SubTasks.AddAsync(data);
        await _db.SaveChangesAsync();

        var userTask = await _db.UserTask.Where(x => x.TaskId == subTask.TaskId).ToListAsync();
        foreach (var item in userTask)
        {
            item.hasUpdate = true;
            _db.UserTask.Update(item);
        }
        await _db.SaveChangesAsync();

        return new BaseResponse<bool>(true);
    }


    public async Task<BaseResponse<ICollection<GetSubTaskDto>>> GetByTaskDone(long taskId)
    {
        if (taskId <= 0)
            return new BaseResponse<ICollection<GetSubTaskDto>>(null);

        var data = await _db.SubTasks.Where(x => x.IsCompleted && !x.IsDeleted && x.TaskId == taskId).OrderByDescending(x => x.CreateAt).ToListAsync();
        var callDtos = new List<GetSubTaskDto>();

        foreach (var item in data)
        {
            var user = await _db.Users.SingleOrDefaultAsync(x => x.Id == item.UserId);

            var dto = new GetSubTaskDto
            {
                Id = item.Id,
                CreateAt = item.CreateAt,
                DeadLine = item.DeadLine,
                IsDeleted = item.IsDeleted,
                IsCompleted = item.IsCompleted,
                Name = item.Name,
                Priority = item.Priority,
                TaskId = item.TaskId,
                UserId = item.UserId,
                UserName = user.FullName
            };

            callDtos.Add(dto); 
        }

        return new BaseResponse<ICollection<GetSubTaskDto>>(callDtos);
    }


    public async Task<BaseResponse<GetSubTaskDto>> GetById(long id)
    {
        if (id <= 0)
            return new BaseResponse<GetSubTaskDto>(null);

        var data = await _db.SubTasks.SingleOrDefaultAsync(x => !x.IsDeleted && x.Id == id);
        if (data == null)
            return new BaseResponse<GetSubTaskDto>(null);

        var user = await _db.Users.SingleOrDefaultAsync(x => x.Id == data.UserId);

        var dto = new GetSubTaskDto
        {
            Id = data.TaskId,
            CreateAt = data.CreateAt,
            DeadLine = data.DeadLine,
            IsDeleted = data.IsDeleted,
            IsCompleted = data.IsCompleted,
            Name = data.Name,
            Priority = data.Priority,
            TaskId = data.TaskId,
            UserId = data.UserId,
            UserName = user.FullName
        };


        return new BaseResponse<GetSubTaskDto>(dto);
    }


    public async Task<BaseResponse<ICollection<GetSubTaskDto>>> GetByTaskNotDone(long taskId)
    {
        if (taskId <= 0)
            return new BaseResponse<ICollection<GetSubTaskDto>>(null);

        var data = await _db.SubTasks.Where(x => !x.IsCompleted && !x.IsDeleted && x.TaskId == taskId).OrderByDescending(x => x.CreateAt).ToListAsync();
        var callDtos = new List<GetSubTaskDto>();

        foreach (var item in data)
        {
            var user = await _db.Users.SingleOrDefaultAsync(x => x.Id == item.UserId);

            var dto = new GetSubTaskDto
            {
                Id = item.Id,
                CreateAt = item.CreateAt,
                DeadLine = item.DeadLine,
                IsDeleted = item.IsDeleted,
                IsCompleted = item.IsCompleted,
                Name = item.Name,
                Priority = item.Priority,
                TaskId = item.TaskId,
                UserId = item.UserId,
                UserName = user.FullName
            };

            callDtos.Add(dto);
        }

        return new BaseResponse<ICollection<GetSubTaskDto>>(callDtos);
    }

  
    public async Task<BaseResponse<bool>> Complete(long id)
    {
        if (id <= 0)
            return new BaseResponse<bool>(null);

        var data = await _db.SubTasks.SingleOrDefaultAsync(x => x.Id == id);
        if (data == null)
            return new BaseResponse<bool>(null);

        data.IsCompleted = !data.IsCompleted;

        _db.SubTasks.Update(data);

        var userTask = _db.UserTask.Where(x => x.Id == data.TaskId);
        foreach (var item in userTask)
        {
            item.hasUpdate = true;
            _db.UserTask.UpdateRange(item);
        }

        await _db.SaveChangesAsync();

        return new BaseResponse<bool>(true);
    }


    public async Task<BaseResponse<bool>> Update(long id, UpdateSubTaskDto subTask)
    {
        if (id <= 0)
            return new BaseResponse<bool>(null);

        var data = await _db.SubTasks.SingleOrDefaultAsync(x => x.Id == id);
        if (data == null)
            return new BaseResponse<bool>(null);

        data.Priority = subTask.Priority;
        data.DeadLine = subTask.DeadLine;
        data.Name = subTask.Name;

        _db.SubTasks.Update(data);

        var userTask = _db.UserTask.Where(x => x.Id == data.TaskId);
        foreach (var item in userTask)
        {
            item.hasUpdate = true;
            _db.UserTask.UpdateRange(item);
        }

        await _db.SaveChangesAsync();

        return new BaseResponse<bool>(true);
    }


    public async Task<BaseResponse<bool>> Remove(long id)
    {
        if (id <= 0)
            return new BaseResponse<bool>(null);

        var data = await _db.SubTasks.SingleOrDefaultAsync(x => x.Id == id);
        if (data == null)
            return new BaseResponse<bool>(null);

        data.IsDeleted = true;

        _db.SubTasks.Update(data);

        var userTask = _db.UserTask.Where(x => x.Id == data.TaskId);
        foreach (var item in userTask)
        {
            item.hasUpdate = true;
            _db.UserTask.UpdateRange(item);
        }

        await _db.SaveChangesAsync();

        return new BaseResponse<bool>(true);
    }
}
