namespace GGR.Shared.FileRecord;

public class UploadFileRecordRequest
{
    public string FileContentBase64 { get; set; } = string.Empty;
    public DateTime DateForRecord { get; set; } = DateTime.Now;
}
