using GGR.Client.Areas.SaleTickets.Services.Contracts;
using GGR.Shared;
using GGR.Shared.Reward;
using GGR.Shared.SaleTicket;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace GGR.Client.Areas.SaleTickets.Services;

public class SaleTicketClientService : ISaleTicketClientService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<SaleTicketClientService> _logger;
    private readonly ILocalStorageService _localStorageService;
    private readonly NavigationManager _navigationManager;

    public SaleTicketClientService(
        HttpClient httpClient,
        ILogger<SaleTicketClientService> logger,
        ILocalStorageService localStorageService,
        NavigationManager navigationManager)
    {
        _navigationManager = navigationManager;
        _httpClient = httpClient;
        _logger = logger;
        _localStorageService = localStorageService;
    }

    public async Task<ServiceResponse<GetSaleTicketsResponse>> GetSaleTicketsByUserEmail(string email)
    {
        _logger.LogInformation("Fetching sale ticket for user email {Email}", email);
        try
        {

            var token = await _localStorageService.GetItemAsync<string>("token");
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"api/SaleTicket/get-tickets-by-email/{email}");
            requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(requestMessage);

            if ( response.StatusCode == System.Net.HttpStatusCode.Unauthorized )
                _navigationManager.NavigateTo(Routes.User.LoginPageSesionExpired);

            var content = await response.Content.ReadFromJsonAsync<ServiceResponse<GetSaleTicketsResponse>>();

            if ( content != null )
            {
                return content;
            }
            else
            {
                throw new Exception("Content for fetching ticket by email is null");
            }
        }
        catch ( Exception ex )
        {
            throw new Exception($"{ex.Message}");
        }
    }

    public async Task<ServiceResponse<GetTotalTicketsCount>> GetTotalTicketsCount()
    {
        _logger.LogInformation("Fetching total sale tickets count");

        try
        {
            var token = await _localStorageService.GetItemAsync<string>("token");
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "api/SaleTicket/get-total-tickets-count");
            requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(requestMessage);

            if ( response.StatusCode == System.Net.HttpStatusCode.Unauthorized )
                _navigationManager.NavigateTo(Routes.User.LoginPageSesionExpired);

            var content = await response.Content.ReadFromJsonAsync<ServiceResponse<GetTotalTicketsCount>>();

            if ( content != null )
            {
                return content;
            }
            else
            {
                throw new Exception("Content for fetching total tickets counts is null");
            }
        }
        catch ( Exception ex )
        {
            throw new Exception($"{ex.Message}");
        }
    }

    public async Task<ServiceResponse<RegisterTicketResponse>> RegisterTicket(RegisterTicketRequest request)
    {
        _logger.LogInformation("Register ticket {TicketFolio} for customer {CustomerId}", request.Folio, request.UserId);

        try
        {
            var token = await _localStorageService.GetItemAsync<string>("token");
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "api/SaleTicket/register-ticket");
            requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            requestMessage.Content = JsonContent.Create(request);

            var response = await _httpClient.SendAsync(requestMessage);

            if ( response.StatusCode == System.Net.HttpStatusCode.Unauthorized )
                _navigationManager.NavigateTo(Routes.User.LoginPageSesionExpired);

            var content = await response.Content.ReadFromJsonAsync<ServiceResponse<RegisterTicketResponse>>();

            if ( content != null )
            {
                return content;
            }
            else
            {
                throw new Exception("Content for fetching total tickets counts is null");
            }

        }
        catch ( Exception ex )
        {
            throw new Exception($"{ex.Message}");
        }
    }
}
