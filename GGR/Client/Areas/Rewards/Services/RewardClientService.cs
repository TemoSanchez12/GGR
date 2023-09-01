using GGR.Client.Areas.Rewards.Services.Contracts;
using GGR.Shared;
using GGR.Shared.Reward;
using GGR.Shared.SaleTicket;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace GGR.Client.Areas.Rewards.Services;

public class RewardClientService : IRewardClientService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<RewardClientService> _logger;
    private readonly ILocalStorageService _localStorageService;
    private readonly NavigationManager _navigationManager;

    public RewardClientService(
        HttpClient httpClient,
        ILogger<RewardClientService> logger,
        ILocalStorageService localStorageService,
        NavigationManager navigationManager)
    {
        _navigationManager = navigationManager;
        _httpClient = httpClient;
        _logger = logger;
        _localStorageService = localStorageService;
    }

    public async Task<ServiceResponse<GetAllRewardsReponse>> GetAllRewards()
    {
        _logger.LogInformation("Fetching all rewards");

        var response = await _httpClient.GetAsync("api/Reward/get-all");
        var content = await response.Content.ReadFromJsonAsync<ServiceResponse<GetAllRewardsReponse>>();

        if ( content != null )
        {
            return content;
        }
        else
        {
            throw new Exception("Content for fetching rewards is null");
        }
    }

    public async Task<ServiceResponse<GetRewardResponse>> GetReward(string rewardId)
    {
        _logger.LogInformation($"Fetching reward with id: {rewardId}");

        var response = await _httpClient.GetAsync($"api/Reward/get/{rewardId}");
        var content = await response.Content.ReadFromJsonAsync<ServiceResponse<GetRewardResponse>>();

        if ( content != null )
        {
            return content;
        }
        else
        {
            throw new Exception($"Content for fetching reward with {rewardId} is null");
        }
    }

    public async Task<ServiceResponse<CreateRewardResponse>> CreateReward(CreateRewardRequest request)
    {
        _logger.LogInformation("Sending request to create reward for email admin {EmailAdmin}", "email.admin");

        var token = await _localStorageService.GetItemAsync<string>("token");
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, "api/Reward/create-reward");
        requestMessage.Content = JsonContent.Create(request);
        requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        try
        {
            var response = await _httpClient.SendAsync(requestMessage);
            var content = await response.Content.ReadFromJsonAsync<ServiceResponse<CreateRewardResponse>>();

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
            return new ServiceResponse<CreateRewardResponse>();
        }
    }

    public async Task<ServiceResponse<UpdateRewardResponse>> UpdateReward(UpdateRewardRequest request)
    {
        _logger.LogInformation("Sending request to create reward for email admin {EmailAdmin}", "email.admin");

        var token = await _localStorageService.GetItemAsync<string>("token");
        var requestMessage = new HttpRequestMessage(HttpMethod.Put, "api/Reward/update-reward")
        {
            Content = JsonContent.Create(request),
            Headers =
            {
                Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token)
            }
        };

        try
        {
            var response = await _httpClient.SendAsync(requestMessage);
            var content = await response.Content.ReadFromJsonAsync<ServiceResponse<UpdateRewardResponse>>();

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
            return new ServiceResponse<UpdateRewardResponse>();
        }
    }

    public async Task DeleteReward(DeleteRewardRequest request)
    {
        _logger.LogInformation($"Sending request to delete reward with id: {request.RewardId}");
        var token = await _localStorageService.GetItemAsync<string>("token");
        var requestMessage = new HttpRequestMessage(HttpMethod.Put, "api/Reward/update-reward");
        requestMessage.Content = JsonContent.Create(request);
        requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        try
        {
            var response = await _httpClient.SendAsync(requestMessage);
        }
        catch ( Exception ex )
        {
            _logger.LogError(ex, "Something went wrong while fetching user by id");
            _navigationManager.NavigateTo(Routes.Customer.LoginCustomerSessionExpired);
        }
    }
}