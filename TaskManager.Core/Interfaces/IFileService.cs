using System.Web.Mvc;
using TaskManager.Core.Dto.Files;
using TeleSales.Core.Responses;

namespace TaskManager.Core.Interfaces;

public interface IFileService
{
    Task<BaseResponse<bool>> UploadFile(Stream fileStream, CreateFileDto dto);
    Task<FileStreamResult> DownloadFile(long id);
    Task<BaseResponse<ICollection<GetFileDto>>> ListFilesAsync(long taskId);
    Task<BaseResponse<bool>> DeleteFile(long id);
}
