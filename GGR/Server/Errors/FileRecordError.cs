
namespace GGR.Server.Errors;

public enum FileRecordError
{
    FileAlreadyUploadedToday,
    ErrorSavingFileRecordToDatabase,
    ErrorSendingEmail,
    FileNotFound,
    FileIsNotCsv,
    FileAlreadyUploadedForThatDate,
    FileDatePassToday
}
