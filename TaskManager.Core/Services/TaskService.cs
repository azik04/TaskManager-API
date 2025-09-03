using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Dto.Tasks;
using TaskManager.Core.Interfaces;
using TaskManager.DataProvider.Context;
using TaskManager.DataProvider.Entities;
using TeleSales.Core.Responses;

namespace TaskManager.Core.Services;

public class TaskService : ITaskService
{
    private readonly ApplicationDbContext _db;
    public TaskService(ApplicationDbContext db)
    {
        _db = db;
    }



    public async Task<BaseResponse<bool>> Create([FromBody] CreateTaskDto task, long userId)
    {
        var theme = await _db.Themes.SingleOrDefaultAsync(x => x.Id == task.ThemeId);
        if (theme == null)
            return new BaseResponse<bool>(false, false, "Theme does not exist");

        var newTask = new Tasks
        {
            TaskName = task.TaskName,
            TaskDescription = task.TaskDescription,
            StatusId = task.StatusId,
            PriorityId = task.PriorityId,
            DeadLine = task.DeadLine,
            ThemeId = task.ThemeId,
            ExecutiveUserId = task.ExecutiveUserId,
            CreateAt = DateTime.UtcNow,
            Contact = task.Contact
        };

        await _db.Tasks.AddAsync(newTask);

        var userTask = _db.UserTask.Where(x => x.TaskId == newTask.Id).AsQueryable();
        foreach (var item in userTask)
        {
            item.hasUpdate = true;
            _db.UserTask.Update(item);
        }

        await _db.SaveChangesAsync();

        return new BaseResponse<bool>(true, true, "Task created successfully.");
    }


    public async Task<BaseResponse<ICollection<GetTaskDto>>> GetAllAsync(long themeId, int statusId)
    {
        if (themeId <= 0)
            return new BaseResponse<ICollection<GetTaskDto>>(null);

        var data = await _db.Tasks
            .Where(x => !x.IsDeleted && x.ThemeId == themeId && x.StatusId == statusId)
            .OrderByDescending(x => x.CreateAt)
            .ToListAsync();

        var dtoList = new List<GetTaskDto>();

        foreach (var item in data)
        {
            var dto = new GetTaskDto
            {
                Id = item.Id,
                DeadLine = item.DeadLine,
                TaskDescription = item.TaskDescription,
                CreateAt = item.CreateAt,
                ExecutiveUserId = item.ExecutiveUserId,
                PriorityId = item.PriorityId,
                StatusId = item.StatusId,
                ThemeId = item.ThemeId,
                IsCompleted = item.IsCompleted,
                Contact = item.Contact,
                isDeleted = item.IsDeleted,
                DateOfCompletion = item.DateOfCompletion,
                TaskName = item.TaskName,
                ExecutiveUserName = item.ExecutiveUser.FullName + " Icraci"
            };

            dtoList.Add(dto);
        }

        return new BaseResponse<ICollection<GetTaskDto>>(dtoList);
    }


    public async Task<BaseResponse<GetTaskDto>> GetById(long id, long userId)
    {
        if (id <= 0 || userId <= 0)
            return new BaseResponse<GetTaskDto>(null);

        var task = await _db.Tasks.SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        if (task == null)
            return new BaseResponse<GetTaskDto>(null);

        var userTask = await _db.UserTask.FirstOrDefaultAsync(x => x.TaskId == task.Id && x.UserId == userId);
        if (userTask != null)
        {
            userTask.hasUpdate = false;
            _db.UserTask.Update(userTask);
            await _db.SaveChangesAsync();
        }

        var dto = new GetTaskDto
        {
            Id = task.Id,
            DeadLine = task.DeadLine,
            TaskDescription = task.TaskDescription,
            CreateAt = task.CreateAt,
            ExecutiveUserId = task.ExecutiveUserId,
            PriorityId = task.PriorityId,
            StatusId = task.StatusId,
            ThemeId = task.ThemeId,
            IsCompleted = task.IsCompleted,
            Contact = task.Contact,
            isDeleted = task.IsDeleted,
            DateOfCompletion = task.DateOfCompletion,
            TaskName = task.TaskName,
            isUpdated = userTask.hasUpdate
        };

        return new BaseResponse<GetTaskDto>(dto);
    }


    public async Task<BaseResponse<bool>> Update(long id, UpdateTaskDto task)
    {
        if (id <= 0)
            return new BaseResponse<bool>(null);

        var data = await _db.Tasks.SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        if (data == null)
            return new BaseResponse<bool>(null);

        data.PriorityId = task.PriorityId;
        data.StatusId = task.StatusId;
        data.DeadLine = task.DeadLine;
        data.TaskDescription = task.TaskDescription;
        data.TaskName = task.TaskName;
        data.ExecutiveUserId = task.ExecutiveUserId;

        _db.Tasks.Update(data);

        var userTask = _db.UserTask.Where(x => x.Id == id).AsQueryable();
        foreach (var item in userTask)
        {
            item.hasUpdate = true;
        }
        _db.UserTask.UpdateRange(userTask);
        await _db.SaveChangesAsync();

        return new BaseResponse<bool>(true);
    }


    public async Task<BaseResponse<bool>> Complite(long taskId)
    {
        if (taskId <= 0)
            return new BaseResponse<bool>(null);

        var data = await _db.Tasks.SingleOrDefaultAsync(x => x.Id == taskId && !x.IsDeleted);
        if (data == null)
            return new BaseResponse<bool>(null);


        switch (data.StatusId)
        {
            case 1:
                data.StatusId = 2;
                break;

            case 2:
                data.StatusId = 3;
                break;

            case 3:
                data.StatusId = 1;
                break;

            default:
                return new BaseResponse<bool>(false);
        }

        _db.Tasks.Update(data);

        var userTask = _db.UserTask.Where(x => x.Id == taskId).AsQueryable();
        foreach (var item in userTask)
        {
            item.hasUpdate = true;
        }
        _db.UserTask.UpdateRange(userTask);

        await _db.SaveChangesAsync();

        return new BaseResponse<bool>(true);
    }


    public async Task<BaseResponse<bool>> Remove(long id)
    {
        if (id <= 0)
            return new BaseResponse<bool>(null);

        var data = await _db.Tasks.SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        if (data == null)
            return new BaseResponse<bool>(null);

        data.IsDeleted = true;

        _db.Tasks.Update(data);
        await _db.SaveChangesAsync();
       
        return new BaseResponse<bool>(true);
    }

}
