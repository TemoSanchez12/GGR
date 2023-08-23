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
        catch (Exception ex)
        {
            _logger.LogError("Something went wrong while saving file {ErrorMessage}", ex.Message);
            var error = (FileRecordError)Enum.Parse(typeof(FileRecordError), ex.Message);
            response.Success = false;

            switch (error)
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
            if (fileName != null)
                response.Data = new GetFileByDateResponse { FileName = fileName };
            return Ok(response);

        }
        catch (Exception ex)
        {
            _logger.LogError("Something went wrong while getting file by date {ErrorMessage}", ex.Message);
            response.Success = false;
            response.Message = _errorFetchingFile;

            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }
}
