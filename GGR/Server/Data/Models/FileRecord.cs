
using GGR.Shared.FileRecord.Definitions;
using System.Diagnostics.Eventing.Reader;

namespace GGR.Server.Data.Models;

public class FileRecord
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileStorageName { get; set; } = string.Empty;
    public string key { get; set; } = string.Empty;
    public DateTime UploadedOn { get; set; } = DateTime.Now;
    public bool IsProcessed { get; set; } = false;

    public FileRecordDefinition ToDefinition()
    {
        return new FileRecordDefinition
        {
            Id = Id,
            FileName = FileName,
            FileStorageName = FileStorageName,
            key = key,
            UploadedOn = UploadedOn,
            IsProcessed = IsProcessed
        };
    }
}
