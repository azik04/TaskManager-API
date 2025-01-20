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

    public async Task<BaseResponse<GetTaskDto>> Create([FromBody] CreateTaskDto task)
    {
        var th = await _db.Themes.SingleOrDefaultAsync(x => x.Id == task.ThemeId);
        if (th == null)
            return new BaseResponse<GetTaskDto>(null, false, "Theme aint exists");

        var data = new Tasks
        {
            TaskName = task.TaskName,
            TaskDescription = task.TaskDescription,
            Status = task.Status,
            Priority = task.Priority,
            DeadLine = task.DeadLine,
            ThemeId = task.ThemeId,
            ExecutiveUserId = task.ExecutiveUserId,
            CreateAt = DateTime.Now,
            UserId = task.UserId,
            Contact = task.Contact
        };

        await _db.Tasks.AddAsync(data);
        await _db.SaveChangesAsync();

        foreach (var item in data.UserId)
        {
            var userExists = await _db.Users.AnyAsync(x => x.Id == item);
            if (!userExists)
                return new BaseResponse<GetTaskDto>(null, false, $"User with ID {item} does not exist");


            var msg = new UserTasks
            {
                CreateAt = DateTime.Now,
                TaskId = data.Id,
                isSeen = false,
                UserId = item,
            };
            await _db.UserTask.AddAsync(msg);
            await _db.SaveChangesAsync();
        }
      
        var user = await _db.Users.SingleOrDefaultAsync(x => x.Id == data.ExecutiveUserId);
        var users = await _db.Users.Where(x => data.UserId.Contains(x.Id)).ToListAsync();
    
        var dto = new GetTaskDto
        {
            Id = data.Id,
            DeadLine = data.DeadLine,
            TaskDescription = data.TaskDescription,
            CreateAt = data.CreateAt,
            ExecutiveUserId = data.ExecutiveUserId,
            Priority = data.Priority.ToString(),
            Status = data.Status.ToString(),
            ThemeId = data.ThemeId,
            UserId = data.UserId,
            IsCompleted = data.IsCompleted,
            Contact = data.Contact,
            isDeleted = data.IsDeleted,
            DateOfCompletion = data.DateOfCompletion,
            TaskName = data.TaskName,
            ExecutiveUserName = user?.FullName,
            UserNames = users.Select(u => u.FullName).ToList()
        };

        return new BaseResponse<GetTaskDto>(dto);
    }

    public async Task<BaseResponse<ICollection<GetTaskDto>>> GetAllDone(long themeId , long userId)
    {
        if (themeId <= 0 || userId <= 0)
            return new BaseResponse<ICollection<GetTaskDto>>(null);

        var data = await _db.Tasks.Where(x => x.IsCompleted && !x.IsDeleted && x.ThemeId == themeId).OrderByDescending(x => x.CreateAt).ToListAsync(); 
        var dtos = new List<GetTaskDto>();

        var not = await _db.UserTask.SingleOrDefaultAsync(x => x.UserId == userId);

        foreach (var item in data)
        {
            var msg = await _db.UserTask.Where(x => x.TaskId == item.Id).ToListAsync();
            var user = await _db.Users.SingleOrDefaultAsync(x => x.Id == item.ExecutiveUserId);
            var users = await _db.Users.Where(x => item.UserId.ToList().Contains(x.Id)).ToListAsync();

            var dto = new GetTaskDto
            {
                Id = item.Id,
                DeadLine = item.DeadLine,
                TaskDescription = item.TaskDescription,
                CreateAt = item.CreateAt,
                ExecutiveUserId = item.ExecutiveUserId,
                Priority = item.Priority.ToString(),
                Status = item.Status.ToString(),
                ThemeId = item.ThemeId,
                UserId = item.UserId,
                IsCompleted = item.IsCompleted,
                Contact = item.Contact,
                isDeleted = item.IsDeleted,
                DateOfCompletion = item.DateOfCompletion,
                TaskName = item.TaskName,
                ExecutiveUserName = user?.FullName,
                UserNames = users.Select(u => u.FullName).ToList(),
                isSeen = not.isSeen

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

        var not = await _db.UserTask.SingleOrDefaultAsync(x => x.UserId == userId);
        foreach (var item in data)
        {
            var user = await _db.Users.SingleOrDefaultAsync(x => x.Id == item.ExecutiveUserId);
            var users = await _db.Users.Where(x => item.UserId.Contains(x.Id)).ToListAsync();
            var dto = new GetTaskDto
            {
                Id = item.Id,
                DeadLine = item.DeadLine,
                TaskDescription = item.TaskDescription,
                CreateAt = item.CreateAt,
                ExecutiveUserId = item.ExecutiveUserId,
                Priority = item.Priority.ToString(),
                Status = item.Status.ToString(),
                ThemeId = item.ThemeId,
                UserId = item.UserId,  
                IsCompleted = item.IsCompleted,
                Contact = item.Contact,
                isDeleted = item.IsDeleted,
                DateOfCompletion = item.DateOfCompletion,
                TaskName = item.TaskName,
                ExecutiveUserName = user?.FullName, 
                UserNames = users.Select(u => u.FullName).ToList(),
                isSeen = not.isSeen
            };

            dtos.Add(dto);
        }

        return new BaseResponse<ICollection<GetTaskDto>>(dtos);
    }



    public async Task<BaseResponse<GetTaskDto>> GetById(long id, long userId)
    {
        if (id <= 0 || userId <= 0)
            return new BaseResponse<GetTaskDto>(null);

        var data = await _db.Tasks.SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        if (data == null)
            return new BaseResponse<GetTaskDto>(null);

        var msg = await _db.UserTask.SingleOrDefaultAsync(x => x.TaskId == data.Id && x.UserId == userId);
        msg.isSeen = false;

        _db.UserTask.Update(msg);
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
            Priority = data.Priority.ToString(),
            Status = data.Status.ToString(),
            ThemeId = data.ThemeId,
            UserId = data.UserId,
            IsCompleted = data.IsCompleted,
            Contact = data.Contact,
            isDeleted = data.IsDeleted,
            DateOfCompletion = data.DateOfCompletion,
            TaskName = data.TaskName,
            ExecutiveUserName = user?.FullName,
            UserNames = users.Select(u => u.FullName).ToList()
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
            Priority = data.Priority.ToString(),
            Status = data.Status.ToString(),
            ThemeId = data.ThemeId,
            UserId = data.UserId,
            IsCompleted = data.IsCompleted,
            Contact = data.Contact,
            isDeleted = data.IsDeleted,
            DateOfCompletion = data.DateOfCompletion,
            TaskName = data.TaskName,
            ExecutiveUserName = user?.FullName,
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
            Priority = data.Priority.ToString(),
            Status = data.Status.ToString(),
            ThemeId = data.ThemeId,
            UserId = data.UserId,
            IsCompleted = data.IsCompleted,
            Contact = data.Contact,
            isDeleted = data.IsDeleted,
            DateOfCompletion = data.DateOfCompletion,
            TaskName = data.TaskName,
            ExecutiveUserName = user?.FullName,
            UserNames = users.Select(u => u.FullName).ToList()
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

        data.IsCompleted = !data.IsCompleted;

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
            Priority = data.Priority.ToString(),
            Status = data.Status.ToString(),
            ThemeId = data.ThemeId,
            UserId = data.UserId,
            IsCompleted = data.IsCompleted,
            Contact = data.Contact,
            isDeleted = data.IsDeleted,
            DateOfCompletion = data.DateOfCompletion,
            TaskName = data.TaskName,
            ExecutiveUserName = user?.FullName,
            UserNames = users.Select(u => u.FullName).ToList()
        };

        return new BaseResponse<GetTaskDto>(dto);
    }
}
