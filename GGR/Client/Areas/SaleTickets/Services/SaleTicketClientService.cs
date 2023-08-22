using GGR.Client.Areas.SaleTickets.Services.Contracts;
using GGR.Shared;
using GGR.Shared.SaleTicket;
using System.Net.Http.Json;

namespace GGR.Client.Areas.SaleTickets.Services;

public class SaleTicketClientService : ISaleTicketClientService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<SaleTicketClientService> _logger;
    private readonly ILocalStorageService _localStorageService;

    public SaleTicketClientService(HttpClient httpClient, ILogger<SaleTicketClientService> logger, ILocalStorageService localStorageService)
    {
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
            var content = await response.Content.ReadFromJsonAsync<ServiceResponse<GetSaleTicketsResponse>>();

            if (content != null)
            {
                return content;
            }
            else
            {
                throw new Exception("Content for fetching ticket by email is null");
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"{ex.Message}");
        }
    }
}
