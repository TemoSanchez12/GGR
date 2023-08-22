

namespace GGR.Server.Commands.Contracts;

public interface IFileRecordCommands
{
    Task<(string, string)> UploadFile(IFormFile file);
    Task<string> GetFileByDate(DateTime date);
}
