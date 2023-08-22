
using GGR.Shared;
using GGR.Shared.FileRecord;

namespace GGR.Client.Areas.FileRecord.Services.Contracts;

public interface IFileRecordClientService
{
  Task<ServiceResponse<UploadFileResponse>> UploadFile(MultipartFormDataContent file);
  Task<ServiceResponse<GetFileByDateResponse>> GetFileRecordByDate(DateTime date);
}
