using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Web.Mvc;
using TaskManager.Core.Dto.Files;
using TaskManager.Core.Interfaces;
using TaskManager.DataProvider.Context;
using TaskManager.DataProvider.Entities;
using TeleSales.Core.Responses;

namespace TaskManager.Core.Services;

public class FileService : IFileService
{
    private readonly ApplicationDbContext _db;
    private readonly IHostEnvironment _env;

    public FileService(ApplicationDbContext dbContext, IHostEnvironment env)
    {
        _db = dbContext;
        _env = env;
    }

    public async Task<BaseResponse<GetFileDto>> DeleteFile(long id)
    {
        if (id <= 0)
            return new BaseResponse<GetFileDto>(null);

        var data = await _db.Files.SingleOrDefaultAsync(f => f.Id == id);
        if (data == null)
            return new BaseResponse<GetFileDto>(null);

        data.IsDeleted = true;

        _db.Files.Update(data);
        await _db.SaveChangesAsync();

        var dto = new GetFileDto
        {
            Id = data.Id,
            FileName = data.FileName,
            IsDeleted = data.IsDeleted,
            CreateAt = DateTime.Now,
            TaskId = data.TaskId,
        };

        return new BaseResponse<GetFileDto>(dto);
    }

    public async Task<FileStreamResult> DownloadFile(long id)
    {
        try
        {
            if (id <= 0)
                return null;

            var data = await _db.Files.FindAsync(id);
            if (data == null || data.IsDeleted)
                return null;
            
            var localFilePath = Path.Combine(_env.ContentRootPath, "wwwroot", "uploads", data.FileName);
            var stream = new FileStream(localFilePath, FileMode.Open);

            return new FileStreamResult(stream, "application/octet-stream")
            {
                FileDownloadName = data.FileName
            };
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<BaseResponse<ICollection<GetFileDto>>> ListFilesAsync(long taskId)
    {
        if (taskId <= 0)
            return new BaseResponse<ICollection<GetFileDto>>(null);

        var datas = await _db.Files.Where(f => f.TaskId == taskId && !f.IsDeleted).ToListAsync();

        var fileVMs = datas.Select(item => new GetFileDto
        {
            Id = item.Id,
            FileName = item.FileName,
            IsDeleted = item.IsDeleted,
            CreateAt = DateTime.Now,
            TaskId = item.TaskId,
        }).ToList();

        return new BaseResponse<ICollection<GetFileDto>>(fileVMs);
    }

    public async Task<BaseResponse<GetFileDto>> UploadFile(Stream fileStream, CreateFileDto dto)
    {
        try
        {
            var uploadDir = Path.Combine(_env.ContentRootPath, "wwwroot", "uploads");

            if (!Directory.Exists(uploadDir))
                Directory.CreateDirectory(uploadDir);

            var localFilePath = Path.Combine(uploadDir, dto.File.FileName);

            if (File.Exists(localFilePath))
                return new BaseResponse<GetFileDto>(null, true, "File with this name exists");

            using (var fileStreamOutput = new FileStream(localFilePath, FileMode.Create))
                await fileStream.CopyToAsync(fileStreamOutput);

            var file = new Files
            {
                FileName = dto.File.FileName,
                TaskId = dto.TaskId,
                CreateAt = DateTime.Now
            };

            await _db.Files.AddAsync(file);

            var userTask = await _db.UserTask.Where(x => x.TaskId == dto.TaskId).ToListAsync();
            foreach(var item  in userTask)
            {
                item.isSeen = true;
                _db.UserTask.Update(item);
            }
            await _db.SaveChangesAsync();

            var fileDto = new GetFileDto
            {
                Id = file.Id,
                FileName = file.FileName,
                IsDeleted = file.IsDeleted,
                CreateAt = DateTime.Now,
                TaskId = file.TaskId,
            };

            return new BaseResponse<GetFileDto>(fileDto);
        }
        catch (Exception ex)
        {
            return new BaseResponse<GetFileDto>(null, false, $"Error: {ex.Message}");
        }
    }

}
