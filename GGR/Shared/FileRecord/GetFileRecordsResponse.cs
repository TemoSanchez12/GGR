using GGR.Shared.FileRecord.Definitions;

namespace GGR.Shared.FileRecord;

public class GetFileRecordsResponse
{
    public List<FileRecordDefinition> FileRecords { get; set; } = new List<FileRecordDefinition>();
}
