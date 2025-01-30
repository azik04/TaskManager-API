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



    public async Task<BaseResponse<GetTaskDto>> Create([FromBody] CreateTaskDto task, long userId)
    {
        var theme = await _db.Themes.SingleOrDefaultAsync(x => x.Id == task.ThemeId);
        if (theme == null)
            return new BaseResponse<GetTaskDto>(null, false, "Theme does not exist");

        var newTask = new Tasks
        {
            TaskName = task.TaskName,
            TaskDescription = task.TaskDescription,
            Status = task.Status,
            Priority = task.Priority,
            DeadLine = task.DeadLine,
            ThemeId = task.ThemeId,
            ExecutiveUserId = task.ExecutiveUserId,
            CreateAt = DateTime.UtcNow,
            UserId = task.UserId,
            Contact = task.Contact
        };

        await _db.Tasks.AddAsync(newTask);
        await _db.SaveChangesAsync();

        var userTheme = await _db.UserThemes.Where(x => x.ThemeId == task.ThemeId).ToListAsync();

        var userTasks = new List<UserTasks>();

        if (userTheme?.Any() == true)
        {
            userTasks.AddRange(userTheme.Select(item => new UserTasks
            {
                TaskId = newTask.Id,
                UserId = item.UserId,
                CreateAt = DateTime.UtcNow,
                isSeen = true
            }));
        }

        if (task.UserId?.Any() == true)
        {
            userTasks.AddRange(task.UserId.Select(assignedUserId => new UserTasks
            {
                TaskId = newTask.Id,
                UserId = assignedUserId,
                CreateAt = DateTime.UtcNow,
                isSeen = false
            }));
        }

        if (newTask.ExecutiveUserId != null && !userTasks.Any(ut => ut.UserId == newTask.ExecutiveUserId.Value))
        {
            userTasks.Add(new UserTasks
            {
                TaskId = newTask.Id,
                UserId = newTask.ExecutiveUserId.Value,
                CreateAt = DateTime.UtcNow,
                isSeen = false
            });
        }

        await _db.UserTask.AddRangeAsync(userTasks);
        await _db.SaveChangesAsync();

        var executiveUser = await _db.Users.SingleOrDefaultAsync(x => x.Id == newTask.ExecutiveUserId);
        var assignedUsers = await _db.Users.Where(x => userTasks.Select(ut => ut.UserId).Contains(x.Id)).ToListAsync();

        var taskDto = new GetTaskDto
        {
            Id = newTask.Id,
            DeadLine = newTask.DeadLine,
            TaskDescription = newTask.TaskDescription,
            CreateAt = newTask.CreateAt,
            ExecutiveUserId = newTask.ExecutiveUserId,
            Priority = newTask.Priority,
            Status = newTask.Status,
            ThemeId = newTask.ThemeId,
            UserId = task.UserId,
            IsCompleted = newTask.IsCompleted,
            Contact = newTask.Contact,
            isDeleted = newTask.IsDeleted,
            DateOfCompletion = newTask.DateOfCompletion,
            TaskName = newTask.TaskName,
            ExecutiveUserName = executiveUser?.FullName != null
                ? $"{executiveUser.FullName} (Executor)"
                : null,
            UserNames = assignedUsers.Select(u => u.FullName).ToList(),
            isSeen = false
        };

        return new BaseResponse<GetTaskDto>(taskDto, true, "Task created successfully.");
    }




    public async Task<BaseResponse<ICollection<GetTaskDto>>> GetAllDone(long themeId , long userId)
    {
        if (themeId <= 0 || userId <= 0)
            return new BaseResponse<ICollection<GetTaskDto>>(null);

        var data = await _db.Tasks.Where(x => x.IsCompleted && !x.IsDeleted && x.ThemeId == themeId).OrderByDescending(x => x.CreateAt).ToListAsync(); 
        var dtos = new List<GetTaskDto>();

        foreach (var item in data)
        {
            var not = await _db.UserTask.FirstOrDefaultAsync(x => x.TaskId == item.Id && x.UserId == userId);
            var user = await _db.Users.SingleOrDefaultAsync(x => x.Id == item.ExecutiveUserId);
            var users = await _db.Users
                .Where(x => item.UserId != null && item.UserId.Contains(x.Id))
                .ToListAsync();

            var dto = new GetTaskDto
            {
                Id = item.Id,
                DeadLine = item.DeadLine,
                TaskDescription = item.TaskDescription,
                CreateAt = item.CreateAt,
                ExecutiveUserId = item.ExecutiveUserId,
                Priority = item.Priority,
                Status = item.Status,
                ThemeId = item.ThemeId,
                UserId = item.UserId,
                IsCompleted = item.IsCompleted,
                Contact = item.Contact,
                isDeleted = item.IsDeleted,
                DateOfCompletion = item.DateOfCompletion,
                TaskName = item.TaskName,
                ExecutiveUserName = user?.FullName + (user?.FullName != null ? " (Icraci)" : ""),
                UserNames = users.Select(u => u.FullName).ToList(),
                isSeen = not?.isSeen

            };
            dtos.Add(dto);
        }

        return new BaseResponse<ICollection<GetTaskDto>>(dtos);
    }

    public async Task<BaseResponse<ICollection<GetTaskDto>>> GetAllNotDone(long themeId, long userId)
    {
        if (themeId <= 0 || userId <= 0)
            return new BaseResponse<ICollection<GetTaskDto>>(null);

        var data = await _db.Tasks
            .Where(x => !x.IsCompleted && !x.IsDeleted && x.ThemeId == themeId)
            .OrderByDescending(x => x.CreateAt)
            .ToListAsync();

        var dtos = new List<GetTaskDto>();

        foreach (var item in data)
        {
            var not = await _db.UserTask.FirstOrDefaultAsync(x => x.TaskId == item.Id && x.UserId == userId);
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == item.ExecutiveUserId);
            var users = await _db.Users.Where(x => item.UserId.Contains(x.Id)).ToListAsync();

            var dto = new GetTaskDto
            {
                Id = item.Id,
                DeadLine = item.DeadLine,
                TaskDescription = item.TaskDescription,
                CreateAt = item.CreateAt,
                ExecutiveUserId = item.ExecutiveUserId,
                Priority = item.Priority,
                Status = item.Status,
                ThemeId = item.ThemeId,
                UserId = item.UserId,  
                IsCompleted = item.IsCompleted,
                Contact = item.Contact,
                isDeleted = item.IsDeleted,
                DateOfCompletion = item.DateOfCompletion,
                TaskName = item.TaskName,
                ExecutiveUserName = user?.FullName + (user?.FullName != null ? " (Icraci)" : ""),
                UserNames = users.Select(u => u.FullName).ToList(),
                isSeen = not?.isSeen
            };

            dtos.Add(dto);
        }

        return new BaseResponse<ICollection<GetTaskDto>>(dtos);
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
            userTask.isSeen = false;
            _db.UserTask.Update(userTask);
            await _db.SaveChangesAsync();
        }

        var executiveUser = await _db.Users.SingleOrDefaultAsync(x => x.Id == task.ExecutiveUserId);
        var assignedUsers = await _db.Users.Where(x => task.UserId.Contains(x.Id)).ToListAsync();

        var dto = new GetTaskDto
        {
            Id = task.Id,
            DeadLine = task.DeadLine,
            TaskDescription = task.TaskDescription,
            CreateAt = task.CreateAt,
            ExecutiveUserId = task.ExecutiveUserId,
            Priority = task.Priority,
            Status = task.Status,
            ThemeId = task.ThemeId,
            UserId = task.UserId,
            IsCompleted = task.IsCompleted,
            Contact = task.Contact,
            isDeleted = task.IsDeleted,
            DateOfCompletion = task.DateOfCompletion,
            TaskName = task.TaskName,
            ExecutiveUserName = executiveUser?.FullName + (executiveUser?.FullName != null ? " (Icraci)" : ""),
            UserNames = assignedUsers.Select(u => u.FullName).ToList(),
            isSeen = userTask?.isSeen
        };

        return new BaseResponse<GetTaskDto>(dto);
    }


    public async Task<BaseResponse<GetTaskDto>> Remove(long id)
    {
        if (id <= 0)
            return new BaseResponse<GetTaskDto>(null);

        var data = await _db.Tasks.SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        if (data == null)
            return new BaseResponse<GetTaskDto>(null);

        data.IsDeleted = true;

        _db.Tasks.Update(data);
        await _db.SaveChangesAsync();
       
        var msg = await _db.UserTask.Where(x => x.TaskId == data.Id).ToListAsync();
        var user = await _db.Users.SingleOrDefaultAsync(x => x.Id == data.ExecutiveUserId);
        var users = await _db.Users.Where(x => data.UserId.Contains(x.Id)).ToListAsync();

        var dto = new GetTaskDto
        {
            Id = data.Id,
            DeadLine = data.DeadLine,
            TaskDescription = data.TaskDescription,
            CreateAt = data.CreateAt,
            ExecutiveUserId = data.ExecutiveUserId,
            Priority = data.Priority,
            Status = data.Status,
            ThemeId = data.ThemeId,
            UserId = data.UserId,
            IsCompleted = data.IsCompleted,
            Contact = data.Contact,
            isDeleted = data.IsDeleted,
            DateOfCompletion = data.DateOfCompletion,
            TaskName = data.TaskName,
            ExecutiveUserName = user?.FullName + (user?.FullName != null ? " (Icraci)" : ""),
            UserNames = users.Select(u => u.FullName).ToList()
        };

        return new BaseResponse<GetTaskDto>(dto);
    }

    public async Task<BaseResponse<GetTaskDto>> Update(long id, UpdateTaskDto task)
    {
        if (id <= 0)
            return new BaseResponse<GetTaskDto>(null);

        var data = await _db.Tasks.SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        if (data == null)
            return new BaseResponse<GetTaskDto>(null);

        data.Priority = task.Priority;
        data.Status = task.Status;
        data.DeadLine = task.DeadLine;
        data.TaskDescription = task.TaskDescription;
        data.TaskName = task.TaskName;
        data.ExecutiveUserId = task.ExecutiveUserId ;
        data.UserId = task.UserTasks;
       
        _db.Tasks.Update(data);
        await _db.SaveChangesAsync();

        var msg = await _db.UserTask.Where(x => x.TaskId == data.Id).ToListAsync();

        foreach (var item in msg)
        {
            item.isSeen = true;
            _db.UserTask.Update(item);
        }
        await _db.SaveChangesAsync();

        var user = await _db.Users.SingleOrDefaultAsync(x => x.Id == data.ExecutiveUserId);
        var users = await _db.Users.Where(x => data.UserId.Contains(x.Id)).ToListAsync();

        var dto = new GetTaskDto
        {
            Id = data.Id,
            DeadLine = data.DeadLine,
            TaskDescription = data.TaskDescription,
            CreateAt = data.CreateAt,
            ExecutiveUserId = data.ExecutiveUserId,
            Priority = data.Priority,
            Status = data.Status,
            ThemeId = data.ThemeId,
            UserId = data.UserId,
            IsCompleted = data.IsCompleted,
            Contact = data.Contact,
            isDeleted = data.IsDeleted,
            DateOfCompletion = data.DateOfCompletion,
            TaskName = data.TaskName,
            ExecutiveUserName = user?.FullName + (user?.FullName != null ? " (Icraci)" : ""),
            UserNames = users?.Select(u => u.FullName).ToList(),
        };

        return new BaseResponse<GetTaskDto>(dto);
    }
    public async Task<BaseResponse<GetTaskDto>> Complite(long id)
    {
        if (id <= 0)
            return new BaseResponse<GetTaskDto>(null);

        var data = await _db.Tasks.SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        if (data == null)
            return new BaseResponse<GetTaskDto>(null);

        if (data.IsCompleted)
        {
            data.IsCompleted = false;
            data.DateOfCompletion = null;  
        }
        else
        {
            data.IsCompleted = true;
            data.DateOfCompletion = DateTime.Now;  
        }
        _db.Tasks.Update(data);
        await _db.SaveChangesAsync();

        var msg = await _db.UserTask.Where(x => x.TaskId == data.Id).ToListAsync();


        var user = await _db.Users.SingleOrDefaultAsync(x => x.Id == data.ExecutiveUserId);
        var users = await _db.Users.Where(x => data.UserId.Contains(x.Id)).ToListAsync();

        var dto = new GetTaskDto
        {
            Id = data.Id,
            DeadLine = data.DeadLine,
            TaskDescription = data.TaskDescription,
            CreateAt = data.CreateAt,
            ExecutiveUserId = data.ExecutiveUserId,
            Priority = data.Priority,
            Status = data.Status,
            ThemeId = data.ThemeId,
            UserId = data.UserId,
            IsCompleted = data.IsCompleted,
            Contact = data.Contact,
            isDeleted = data.IsDeleted,
            DateOfCompletion = data.DateOfCompletion,
            TaskName = data.TaskName,
            ExecutiveUserName = user?.FullName + (user?.FullName != null ? " (Icraci)" : ""),
            UserNames = users.Select(u => u.FullName).ToList()
        };

        return new BaseResponse<GetTaskDto>(dto);
    }
}
