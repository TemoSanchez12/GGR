using GGR.Shared.FileRecord.Definitions;

namespace GGR.Client.Areas.FileRecord.Utils;

public static class FileRecordMapper
{
    public static Models.FileRecord MapToEntity(FileRecordDefinition definition)
    {
        return new Models.FileRecord
        {
            Id = definition.Id,
            FileName = definition.FileName,
            FileStorageName = definition.FileStorageName,
            IsProcessed = definition.IsProcessed,
            key = definition.key,
            UploadedOn = definition.UploadedOn
        };
    }
}