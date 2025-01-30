using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Dto.Comment;
using TaskManager.Core.Interfaces;
using TaskManager.DataProvider.Context;
using TaskManager.DataProvider.Entities;
using TeleSales.Core.Responses;

namespace TaskManager.Core.Services;

public class CommentService : ICommentService
{
    private readonly ApplicationDbContext _db;
    public CommentService (ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<BaseResponse<GetCommentDto>> Create(CreateCommentDto comment)
    {
        var data = new Comments
        {
            Message = comment.Message,
            TaskId = comment.TaskId,
            UserId = comment.UserId,
            CreateAt = DateTime.Now,
        };

        await _db.Comments.AddAsync(data);

        var useTask = await _db.UserTask.Where(x => x.TaskId == comment.TaskId).ToListAsync();
        foreach (var item in useTask)
        {
            item.isSeen = true;
            _db.UserTask.Update(item);
        }

        await _db.SaveChangesAsync();

        var dto = new GetCommentDto
        {
            Id = data.Id,
            Message = data.Message,
            TaskId = data.TaskId,
            UserId = data.UserId,
            IsDeleted = data.IsDeleted,
            CreateAt = data.CreateAt,
        };

    


        return new BaseResponse<GetCommentDto>(dto);
    }

    public async Task<BaseResponse<ICollection<GetCommentDto>>> GetByTask(long taskId)
    {
        if (taskId <=0)
            return new BaseResponse<ICollection<GetCommentDto>>(null);


        var data = await _db.Comments.Where(x => x.TaskId == taskId && !x.IsDeleted).OrderByDescending(x => x.CreateAt).ToListAsync();
        
        var dtos = new List<GetCommentDto>();

        foreach (var item in data)
        {
            var user = await _db.Users.FirstAsync(x => x.Id == item.UserId);
            var dto = new GetCommentDto
            {
                Id = item.Id,
                Message = item.Message,
                TaskId = item.TaskId,
                UserId = item.UserId,
                IsDeleted = item.IsDeleted,
                CreateAt = item.CreateAt,
                UserName = user.FullName,
            };
            dtos.Add(dto);
        }

        return new BaseResponse<ICollection<GetCommentDto>>(dtos);
    }

    public async Task<BaseResponse<GetCommentDto>> Remove(long id)
    {
        if (id <= 0)
            return new BaseResponse<GetCommentDto>(null);
        
        var data = await _db.Comments.SingleOrDefaultAsync(x => x.Id == id);
        if (data == null)
            return new BaseResponse<GetCommentDto>(null);

        data.IsDeleted = true;

        _db.Update(data);
        await _db.SaveChangesAsync();

        var dto = new GetCommentDto
        {
            Id = data.Id,
            Message = data.Message,
            TaskId = data.TaskId,
            UserId = data.UserId,
            IsDeleted = data.IsDeleted,
            CreateAt = data.CreateAt,
        };

        return new BaseResponse<GetCommentDto>(dto);
    }

    public async Task<BaseResponse<GetCommentDto>> Update(long id, UpdateCommentDto comment)
    {
        if (id <= 0)
            return new BaseResponse<GetCommentDto>(null);

        var data = await _db.Comments.SingleOrDefaultAsync(x => x.Id == id);
        if (data == null)
            return new BaseResponse<GetCommentDto>(null);

        data.Message = comment.Message;
        
        _db.Comments.Update(data);
        await _db.SaveChangesAsync();

        var dto = new GetCommentDto
        {
            Id = data.Id,
            Message = data.Message,
            TaskId = data.TaskId,
            UserId = data.UserId,
            IsDeleted = data.IsDeleted,
            CreateAt = data.CreateAt,
        };

        return new BaseResponse<GetCommentDto>(dto);
    }
}
