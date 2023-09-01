
namespace GGR.Shared.FileRecord.Definitions;

public class FileRecordDefinition
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileStorageName { get; set; } = string.Empty;
    public string key { get; set; } = string.Empty;
    public DateTime UploadedOn { get; set; } = DateTime.Now;
    public bool IsProcessed { get; set; } = false;
}
