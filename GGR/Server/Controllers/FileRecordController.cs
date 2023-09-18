using Microsoft.AspNetCore.Mvc;
using GGR.Server.Commands.Contracts;
using Microsoft.AspNetCore.Authorization;
using GGR.Shared;
using GGR.Shared.FileRecord;
using GGR.Server.Errors;

namespace GGR.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileRecordController : ControllerBase
{
    private static readonly string _successUploadFileMessage = "Archivo guardado correctamente";
    private static readonly string _errorUploadFileMessage = "Error al guardar el archivo";
    private static readonly string _errorFetchingFile = "Error al obtener el archivo";
    private static readonly string _errorFetchingFileWithoutProcessing = "Error al obtener registros sin procesar";
    private static readonly string _successFetchingFileWithoutProcessing = "Se han devulto los registros de archivo sin procesar";
    private static readonly string _successProcessingFileRecordMessage = "Se ha procesaso el archivo de registros correctamente";
    private static readonly string _errorProcessingFileRecordMessage = "Error al procesar el archivo de registro";
    private static readonly string _successDeleteFileRecordMessage = "El archivo de registro se ha elimiado correctamente";
    private static readonly string _errorDeleteFileRecordMessage = "Algo ha salido mal a la hora de elimiar el registro";

    private readonly ILogger<FileRecordController> _logger;
    private readonly IFileRecordCommands _fileRecordCommands;

    public FileRecordController(ILogger<FileRecordController> logger, IFileRecordCommands fileRecordCommands)
    {
        _logger = logger;
        _fileRecordCommands = fileRecordCommands;
    }

    [HttpPost("upload")]
    [Authorize(Roles = "Admin, Editor")]
    public async Task<ActionResult<ServiceResponse<UploadFileResponse>>> UploadFile(IFormFile file)
    {
        var response = new ServiceResponse<UploadFileResponse>();

        try
        {
            var (fileName, StoredFileName) = await _fileRecordCommands.UploadFile(file);
            response.Success = true;
            response.Message = _successUploadFileMessage;
            response.Data = new UploadFileResponse { FileName = fileName, StoredFileName = StoredFileName };
            return Ok(response);
        }
        catch ( Exception ex )
        {
            _logger.LogError("Something went wrong while saving file {ErrorMessage}", ex.Message);
            var error = (FileRecordError) Enum.Parse(typeof(FileRecordError), ex.Message);
            response.Success = false;

            switch ( error )
            {
                case FileRecordError.FileAlreadyUploadedToday:
                    response.Message = FileRecordErrorMessage.FileAlreadyUploadedToday;
                    return BadRequest(response);
                case FileRecordError.ErrorSavingFileRecordToDatabase:
                    response.Message = FileRecordErrorMessage.ErrorSavingFileRecordToDatabase;
                    return StatusCode(StatusCodes.Status500InternalServerError, response);
                case FileRecordError.ErrorSendingEmail:
                    response.Message = FileRecordErrorMessage.ErrorSendingEmail;
                    return StatusCode(StatusCodes.Status500InternalServerError, response);
                default:
                    response.Message = _errorUploadFileMessage;
                    return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }
    }

    [HttpPost("remove-file-record")]
    [Authorize(Roles = "Admin, Editor")]
    public async Task<ActionResult<ServiceResponse<DeleteFileRecordResponse>>> DeleteFileRecord(DeleteFileRecordRequest request)
    {
        var response = new ServiceResponse<DeleteFileRecordResponse>();

        try
        {
            var fileRecord = await _fileRecordCommands.DeleteFileRecord(request);
            response.Success = true;
            response.Message = _successDeleteFileRecordMessage;
            response.Data = new DeleteFileRecordResponse { FileRecord = fileRecord.ToDefinition() };
            return Ok(response);
        }
        catch ( Exception ex )
        {
            _logger.LogError(ex, "Something went wrong while fetching file records without processing");
            response.Success = false;
            response.Message = _errorDeleteFileRecordMessage;
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet("get-file-records-without-processing")]
    [Authorize(Roles = "Admin, Editor")]
    public async Task<ActionResult<ServiceResponse<GetFileRecordsResponse>>> GetFileRecordsWithoutProcessing()
    {
        var response = new ServiceResponse<GetFileRecordsResponse>();

        try
        {
            var fileRecords = await _fileRecordCommands.GetFileRecordsWithoutProcessing();
            response.Success = true;
            response.Message = _successFetchingFileWithoutProcessing;
            response.Data = new GetFileRecordsResponse { FileRecords = fileRecords.Select(f => f.ToDefinition()).ToList() };
            return Ok(response);
        }
        catch ( Exception ex )
        {
            _logger.LogError(ex, "Something went wrong while fetching file records without processing");
            response.Success = false;
            response.Message = _errorFetchingFileWithoutProcessing;
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPost("processing-file-record")]
    [Authorize(Roles = "Admin, Editor")]
    public async Task<ActionResult<ServiceResponse<ProcessingFileRecordResponse>>> ProcessFileRecord(ProcessingFileRecordRequest request)
    {
        var response = new ServiceResponse<ProcessingFileRecordResponse>();

        try
        {
            var fileRecord = await _fileRecordCommands.CheckTicketFromFile(request.FileRecordId);
            response.Success = true;
            response.Message = _successProcessingFileRecordMessage;
            response.Data = new ProcessingFileRecordResponse { FileRecord = fileRecord.ToDefinition() };
            return Ok(response);
        }
        catch ( Exception ex )
        {
            _logger.LogError(ex, "Something went wrong while processing file record");
            response.Success = false;
            response.Message = _errorProcessingFileRecordMessage;
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPost("upload-file-record")]
    [Authorize(Roles = "Admin, Editor")]
    public async Task<ActionResult<ServiceResponse<UploadFileResponse>>> UploadFileRecord(UploadFileRecordRequest request)
    {
        var response = new ServiceResponse<UploadFileResponse>();

        try
        {
            var (fileName, StoredFileName) = await _fileRecordCommands.UploadFileRecord(request);
            response.Success = true;
            response.Message = _successUploadFileMessage;
            response.Data = new UploadFileResponse { FileName = fileName, StoredFileName = StoredFileName };
            return Ok(response);
        }
        catch ( Exception ex )
        {
            _logger.LogError("Something went wrong while saving file {ErrorMessage}", ex.Message);
            var error = (FileRecordError) Enum.Parse(typeof(FileRecordError), ex.Message);
            response.Success = false;

            switch ( error )
            {
                case FileRecordError.FileAlreadyUploadedForThatDate:
                    response.Message = FileRecordErrorMessage.FileAlreadyUploadedForThatDate;
                    return BadRequest(response);
                case FileRecordError.FileDatePassToday:
                    response.Message = FileRecordErrorMessage.FileDatePassToday;
                    return BadRequest(response);
                case FileRecordError.ErrorSavingFileRecordToDatabase:
                    response.Message = FileRecordErrorMessage.ErrorSavingFileRecordToDatabase;
                    return StatusCode(StatusCodes.Status500InternalServerError, response);
                case FileRecordError.ErrorSendingEmail:
                    response.Message = FileRecordErrorMessage.ErrorSendingEmail;
                    return StatusCode(StatusCodes.Status500InternalServerError, response);
                default:
                    response.Message = _errorUploadFileMessage;
                    return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }
    }

    [HttpPost("get-file-by-date")]
    [Authorize(Roles = "Admin, Editor")]
    public async Task<ActionResult<ServiceResponse<GetFileByDateResponse>>> GetFileByDate(GetFileByDateRequest request)
    {
        var response = new ServiceResponse<GetFileByDateResponse>();

        try
        {
            var fileName = await _fileRecordCommands.GetFileByDate(request.Date);
            response.Success = true;
            response.Message = _successUploadFileMessage;
            if ( fileName != null )
                response.Data = new GetFileByDateResponse { FileName = fileName };
            return Ok(response);

        }
        catch ( Exception ex )
        {
            _logger.LogError("Something went wrong while getting file by date {ErrorMessage}", ex.Message);
            response.Success = false;
            response.Message = _errorFetchingFile;

            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }
}
