

using GGR.Server.Data.Models;
using GGR.Shared.FileRecord;

namespace GGR.Server.Commands.Contracts;

public interface IFileRecordCommands
{
    Task<(string, string)> UploadFile(IFormFile file);
    Task<(string, string)> UploadFileRecord(UploadFileRecordRequest request);
    Task<string?> GetFileByDate(DateTime date);
    Task CheckTicketsFromFiles();
    Task<List<FileRecord>> GetFileRecordsWithoutProcessing();
    Task<FileRecord> CheckTicketFromFile(Guid FileRecordId);
    Task<FileRecord> DeleteFileRecord(DeleteFileRecordRequest request);
}
