
namespace GGR.Shared.FileRecord;

public class DeleteFileRecordRequest
{
    public Guid FileRecordId { get; set; } = Guid.NewGuid();
}
