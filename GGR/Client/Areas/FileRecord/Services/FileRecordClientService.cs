using GGR.Client.Areas.FileRecord.Services.Contracts;
using GGR.Shared;
using GGR.Shared.FileRecord;
using GGR.Shared.RewardClaim;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GGR.Client.Areas.FileRecord.Services;

public class FileRecordClientService : IFileRecordClientService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<FileRecordClientService> _logger;
    private readonly ILocalStorageService _localStorageService;
    private readonly NavigationManager _navigationManager;

    public FileRecordClientService(
        HttpClient httpClient,
        ILogger<FileRecordClientService> logger,
        ILocalStorageService localStorageService,
        NavigationManager navigationManager)
    {
        _navigationManager = navigationManager;
        _httpClient = httpClient;
        _logger = logger;
        _localStorageService = localStorageService;
    }

    public async Task<ServiceResponse<UploadFileResponse>> UploadFile(MultipartFormDataContent file)
    {
        _logger.LogInformation("Uploading file in date {Date}", DateTime.Now.ToString("yyyy-MM-dd"));

        var token = await _localStorageService.GetItemAsync<string>("token");
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/FileRecord/upload");
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        requestMessage.Content = file;
        try
        {
            var response = await _httpClient.SendAsync(requestMessage);
            var content = await response.Content.ReadFromJsonAsync<ServiceResponse<UploadFileResponse>>();

            if ( content != null )
            {
                return content;
            }
            else
            {
                throw new Exception();
            }
        }
        catch ( Exception ex )
        {
            _logger.LogError(ex, "Something went wrong while fetching user by id");
            _navigationManager.NavigateTo(Routes.Customer.LoginCustomerSessionExpired);
            return new ServiceResponse<UploadFileResponse>();
        }

    }

    public async Task<ServiceResponse<UploadFileResponse>> UploadFileRecord(UploadFileRecordRequest request)
    {
        _logger.LogInformation("Uploading file in date {Date}", DateTime.Now.ToString("yyyy-MM-dd"));

        var token = await _localStorageService.GetItemAsync<string>("token");
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/FileRecord/upload-file-record");
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        requestMessage.Content = JsonContent.Create(request);
        try
        {
            var response = await _httpClient.SendAsync(requestMessage);
            var content = await response.Content.ReadFromJsonAsync<ServiceResponse<UploadFileResponse>>();

            if ( content != null )
            {
                return content;
            }
            else
            {
                throw new Exception();
            }
        }
        catch ( Exception ex )
        {
            _logger.LogError(ex, "Something went wrong while fetching user by id");
            _navigationManager.NavigateTo(Routes.Customer.LoginCustomerSessionExpired);
            return new ServiceResponse<UploadFileResponse>();
        }

    }

    public async Task<ServiceResponse<GetFileByDateResponse>> GetFileRecordByDate(DateTime date)
    {
        _logger.LogInformation($"Fetching file record with date: {date}");
        var token = await _localStorageService.GetItemAsync<string>("token");
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/FileRecord/get-file-by-date");
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        requestMessage.Content = JsonContent.Create(new GetFileByDateRequest { Date = date });

        try
        {
            var response = await _httpClient.SendAsync(requestMessage);
            var content = await response.Content.ReadFromJsonAsync<ServiceResponse<GetFileByDateResponse>>();

            if ( content != null )
            {
                return content;
            }
            else
            {
                throw new Exception();
            }
        }
        catch ( Exception ex )
        {
            _logger.LogError(ex, "Something went wrong while fetching user by id");
            _navigationManager.NavigateTo(Routes.Customer.LoginCustomerSessionExpired);
            return new ServiceResponse<GetFileByDateResponse>();
        }
    }

    public async Task<ServiceResponse<GetFileRecordsResponse>> GetFileRecordsWithoutProcessing()
    {
        _logger.LogInformation($"Fetching file record without processing {DateTime.Now}");
        var token = await _localStorageService.GetItemAsync<string>("token");
        var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/api/FileRecord/get-file-records-without-processing");
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        try
        {
            var response = await _httpClient.SendAsync(requestMessage);
            var content = await response.Content.ReadFromJsonAsync<ServiceResponse<GetFileRecordsResponse>>();

            if ( content != null )
            {
                return content;
            }
            else
            {
                throw new Exception();
            }
        }
        catch ( Exception ex )
        {
            _logger.LogError(ex, "Something went wrong while fetching user by id");
            _navigationManager.NavigateTo(Routes.Customer.LoginCustomerSessionExpired);
            return new ServiceResponse<GetFileRecordsResponse>();
        }
    }
}
