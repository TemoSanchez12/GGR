
using GGR.Shared.FileRecord.Definitions;

namespace GGR.Shared.FileRecord;

public class DeleteFileRecordResponse
{
    public FileRecordDefinition FileRecord { get; set; } = new FileRecordDefinition();
}
