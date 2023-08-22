
namespace GGR.Server.Data.Models;

public class FileRecord
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileStorageName { get; set; } = string.Empty;
    public string key { get; set; } = string.Empty;
    public DateTime UploadedOn { get; set; } = DateTime.Now;
}
