using GGR.Shared.FileRecord.Definitions;

namespace GGR.Shared.FileRecord;

public class ProcessingFileRecordResponse
{
    public FileRecordDefinition FileRecord { get; set; } = new FileRecordDefinition();
}
