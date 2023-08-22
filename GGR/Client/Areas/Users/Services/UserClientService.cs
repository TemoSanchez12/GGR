using GGR.Client.Areas.Users.Services.Contract;
using GGR.Shared.User;
using GGR.Shared;
using System.Net.Http.Json;
using System.ComponentModel.DataAnnotations;

namespace GGR.Client.Areas.Users.Services;

public class UserClientService : IUserClientService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UserClientService> _logger;
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly ILocalStorageService _localStorageService;

    public UserClientService(
        HttpClient httpClient,
        ILogger<UserClientService> logger,
        AuthenticationStateProvider authenticationStateProvider,
        ILocalStorageService localStorageService)
    {
        _httpClient = httpClient;
        _logger = logger;
        _authenticationStateProvider = authenticationStateProvider;
        _localStorageService = localStorageService;
    }

    public async Task<ServiceResponse<UserRegisterResponse>> RegisterUser(UserRegisterRequest request)
    {
        _logger.LogInformation("Sending register request for user email {UserEmail}", request.Email);

        var response = await _httpClient.PostAsJsonAsync("api/User/register", request);
        var content = await response.Content.ReadFromJsonAsync<ServiceResponse<UserRegisterResponse>>();

        if ( content != null )
        {
            return content;
        }
        else
        {
            throw new Exception($"Content response for register user {request.Email} is null");
        }
    }

    public async Task<ServiceResponse<UserLoginResponse>> UserLogin(UserLoginRequest request)
    {
        _logger.LogInformation("Sending login request for user email {UserEmail}", request.Email);

        var response = await _httpClient.PostAsJsonAsync("api/User/login", request);
        var content = await response.Content.ReadFromJsonAsync<ServiceResponse<UserLoginResponse>>();

        if ( content != null )
        {
            if ( content.Success )
            {
                await _localStorageService.SetItemAsync("token", content.Data!.Token);
                await _authenticationStateProvider.GetAuthenticationStateAsync();
            }
            return content;
        }
        else
            throw new Exception($"Content response for login request for user {request.Email} is null");
    }

    public async Task UserLogout()
    {
        await _localStorageService.RemoveItemAsync("token");
        await _authenticationStateProvider.GetAuthenticationStateAsync();
    }

    public async Task<ServiceResponse<GetUsersResponse>> GetUsersByEmail(string email)
    {
        _logger.LogInformation($"Fetching users that contains email {email}");
        var token = await _localStorageService.GetItemAsync<string>("token");
        var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"api/User/get-by-email/{email}");

        requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(requestMessage);
        var content = await response.Content.ReadFromJsonAsync<ServiceResponse<GetUsersResponse>>();

        if ( content != null )
        {
            return content;
        }
        else
        {
            throw new Exception();
        }
    }

    public async Task VerifyUser(string token)
    {
        _logger.LogInformation("Verifing user token {Token}", token);
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"api/User/verify?token={token}");
        var response = await _httpClient.SendAsync(requestMessage);

        response.EnsureSuccessStatusCode();
    }

    public async Task RestoreVerifyToken(UserRestoreVerifyTokenRequest request)
    {
        _logger.LogInformation("Sending request for restore verify token");
        var response = await _httpClient.PostAsJsonAsync("api/User/restore-verify-token", request);
        response.EnsureSuccessStatusCode();
    }
}
