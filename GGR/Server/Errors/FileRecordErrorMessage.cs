
namespace GGR.Server.Errors;

public static class FileRecordErrorMessage
{
    public static readonly string FileAlreadyUploadedToday = "Ya se ha cargado un archivo hoy";
    public static readonly string ErrorSavingFileRecordToDatabase = "Error al guardar el archivo";
    public static readonly string ErrorSendingEmail = "Error al enviar el email";
    public static readonly string FileNotFound = "Archivo no encontrado";
    public static readonly string FileIsNotCsv = "El archivo no es un csv";
    public static readonly string FileAlreadyUploadedForThatDate = "Un archivo ya se ha cargado para esa fecha";
    public static readonly string FileDatePassToday = "No puede subir archivos con fechas futuras a hoy";
}
