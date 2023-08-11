using GGR.Client.Areas.RewardClaim.Services.Contracts;
using GGR.Shared;
using GGR.Shared.RewardClaim;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace GGR.Client.Areas.RewardClaim.Services;
public class RewardClaimClientService : IRewardClaimClientService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<RewardClaimClientService> _logger;
    private readonly ILocalStorageService _localStorageService;

    public RewardClaimClientService(HttpClient httpClient, ILogger<RewardClaimClientService> logger, ILocalStorageService localStorageService)
    {
        _httpClient = httpClient;
        _logger = logger;
        _localStorageService = localStorageService;
    }

    public async Task<ServiceResponse<GetAllRewardClaimsResponse>> GetAllRewardClaims()
    {
        _logger.LogInformation("Fetching all reward claims");

        var token = await _localStorageService.GetItemAsStringAsync("token");
        var requestMessage = new HttpRequestMessage(HttpMethod.Get, "api/RewardClaims/get-all-reward-claims");
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(requestMessage);
        var content = await response.Content.ReadFromJsonAsync<ServiceResponse<GetAllRewardClaimsResponse>>();

        if ( content != null )
        {
            return content;
        }
        else
        {
            throw new Exception("Content for fetching all reward claims is null");
        }
    }

    public async Task<ServiceResponse<GetAllRewardClaimsResponse>> GetRewardClaimsByEmail(string email)
    {
        _logger.LogInformation("Fetching all reward claims");

        var token = await _localStorageService.GetItemAsStringAsync("token");
        var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"api/RewardClaims/get-reward-claims-email/{email}");
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(requestMessage);
        var content = await response.Content.ReadFromJsonAsync<ServiceResponse<GetAllRewardClaimsResponse>>();

        if ( content != null )
        {
            return content;
        }
        else
        {
            throw new Exception("Content for fetching all reward claims is null");
        }
    }
}
