using GGR.Client.Areas.Users.Services.Contract;
using GGR.Shared.User;
using GGR.Shared;
using System.Net.Http.Json;

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
}
