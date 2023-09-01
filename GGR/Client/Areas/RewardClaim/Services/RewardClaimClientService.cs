using GGR.Client.Areas.RewardClaim.Services.Contracts;
using GGR.Shared;
using GGR.Shared.Reward;
using GGR.Shared.RewardClaim;
using GGR.Shared.User;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace GGR.Client.Areas.RewardClaim.Services;
public class RewardClaimClientService : IRewardClaimClientService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<RewardClaimClientService> _logger;
    private readonly ILocalStorageService _localStorageService;
    private readonly NavigationManager _navigationManager;

    public RewardClaimClientService(
        HttpClient httpClient,
        ILogger<RewardClaimClientService> logger,
        ILocalStorageService localStorageService,
        NavigationManager navigationManager)
    {
        _navigationManager = navigationManager;
        _httpClient = httpClient;
        _logger = logger;
        _localStorageService = localStorageService;
    }

    public async Task<ServiceResponse<GetAllRewardClaimsResponse>> GetAllRewardClaims()
    {
        _logger.LogInformation("Fetching all reward claims");

        var token = await _localStorageService.GetItemAsync<string>("token");
        var requestMessage = new HttpRequestMessage(HttpMethod.Get, "api/RewardClaim/get-all-reward-claims");
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        try
        {
            var response = await _httpClient.SendAsync(requestMessage);
            var content = await response.Content.ReadFromJsonAsync<ServiceResponse<GetAllRewardClaimsResponse>>();

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
            return new ServiceResponse<GetAllRewardClaimsResponse>();
        }
    }

    public async Task<ServiceResponse<UpdateRewardClaimStatusResponse>> UpdateRewardClaimStatus(UpdateRewardClaimStatusRequest request)
    {
        _logger.LogInformation("Updating reward claim status");

        var token = await _localStorageService.GetItemAsync<string>("token");
        var requestMessage = new HttpRequestMessage(HttpMethod.Put, "api/RewardClaim/update-reward-claim-status-admin");
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        requestMessage.Content = JsonContent.Create(request);
        try
        {
            var response = await _httpClient.SendAsync(requestMessage);
            var content = await response.Content.ReadFromJsonAsync<ServiceResponse<UpdateRewardClaimStatusResponse>>();

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
            return new ServiceResponse<UpdateRewardClaimStatusResponse>();
        }
    }

    public async Task<ServiceResponse<GetAllRewardClaimsResponse>> GetRewardClaimsByEmail(string email)
    {
        _logger.LogInformation("Fetching all reward claims");

        var token = await _localStorageService.GetItemAsync<string>("token");
        var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"api/RewardClaim/get-reward-claims-email/{email}");
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        try
        {
            var response = await _httpClient.SendAsync(requestMessage);
            var content = await response.Content.ReadFromJsonAsync<ServiceResponse<GetAllRewardClaimsResponse>>();

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
            return new ServiceResponse<GetAllRewardClaimsResponse>();
        }
    }

    public async Task<ServiceResponse<GetAllRewardClaimsResponse>> GetRewardClaimsById(string id)
    {
        _logger.LogInformation("Fetching all reward claims");

        var token = await _localStorageService.GetItemAsync<string>("token");
        var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"api/RewardClaim/get-reward-claims-by-id/{id}");
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        try
        {
            var response = await _httpClient.SendAsync(requestMessage);
            var content = await response.Content.ReadFromJsonAsync<ServiceResponse<GetAllRewardClaimsResponse>>();

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
            return new ServiceResponse<GetAllRewardClaimsResponse>();
        }
    }

    public async Task<ServiceResponse<CreateRewardClaimResponse>> CreateRewardClaim(CreateRewardClaimRequest request)
    {
        _logger.LogInformation("Creating reward claim for customer {CustomerId} for reward {RewardId}", request.UserId, request.RewardId);

        var token = await _localStorageService.GetItemAsync<string>("token");
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/RewardClaim/create-reward-claim");
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        requestMessage.Content = JsonContent.Create(request);

        try
        {
            var response = await _httpClient.SendAsync(requestMessage);
            var content = await response.Content.ReadFromJsonAsync<ServiceResponse<CreateRewardClaimResponse>>();

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
            _logger.LogError(ex, "Something went wrong while creating reward claim");
            _navigationManager.NavigateTo(Routes.Customer.LoginCustomerSessionExpired);
            return new ServiceResponse<CreateRewardClaimResponse>();
        }
    }
}
