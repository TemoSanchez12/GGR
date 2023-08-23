using GGR.Client.Areas.FileRecord.Services.Contracts;
using GGR.Shared;
using GGR.Shared.FileRecord;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Headers;
using System.Net.Http.Json;

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
        try
        {
            var token = await _localStorageService.GetItemAsync<string>("token");
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/FileRecord/upload");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            requestMessage.Content = file;

            var response = await _httpClient.SendAsync(requestMessage);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                _navigationManager.NavigateTo(Routes.User.LoginPageSesionExpired);


            var content = await response.Content.ReadFromJsonAsync<ServiceResponse<UploadFileResponse>>();

            if (content != null)
            {
                return content;
            }
            else
            {
                throw new Exception("Content for upload file is null");
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error uploading file", ex);
        }
    }

    public async Task<ServiceResponse<GetFileByDateResponse>> GetFileRecordByDate(DateTime date)
    {
        _logger.LogInformation($"Fetching file record with date: {date}");
        var token = await _localStorageService.GetItemAsync<string>("token");
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/FileRecord/get-file-by-date");
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        requestMessage.Content = JsonContent.Create(new GetFileByDateRequest { Date = date });

        var response = await _httpClient.SendAsync(requestMessage);
        var content = await response.Content.ReadFromJsonAsync<ServiceResponse<GetFileByDateResponse>>();

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            _navigationManager.NavigateTo(Routes.User.LoginPageSesionExpired);


        if (content != null)
        {
            return content;
        }
        else
        {
            throw new Exception("Content for get file by date is null");
        }
    }
}
