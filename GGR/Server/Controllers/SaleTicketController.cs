
using GGR.Server.Commands.Contracts;
using GGR.Server.Errors;
using GGR.Shared;
using GGR.Shared.SaleTicket;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GGR.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SaleTicketController : ControllerBase
{
    private static string _genericErrorMessage = "Algo ha salido mal, intentelo de nuevo";
    private static string _successGetTicketMessage = "Se han devuelto los tickets correctamente";
    private static string _successGetTicketsCountMessage = "Se ha devuelto el numero de tickets registrados";
    private static string _successRegisterTicketMessage = "Ticket registrado correctamente";

    private readonly ILogger<SaleTicketController> _logger;
    private readonly ISaleTicketCommands _saleTicketCommands;

    public SaleTicketController(ILogger<SaleTicketController> logger, ISaleTicketCommands saleTicketCommands)
    {
        _logger = logger;
        _saleTicketCommands = saleTicketCommands;
    }

    [HttpGet("get-tickets-by-email/{email}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ServiceResponse<GetSaleTicketsResponse>>> GetSaleTicketsByEmail(string email)
    {
        var response = new ServiceResponse<GetSaleTicketsResponse>();
        try
        {
            var tickets = await _saleTicketCommands.GetTicketsByEmail(email);
            response.Success = true;
            response.Message = _successGetTicketMessage;
            response.Data = new GetSaleTicketsResponse { Tickets = tickets.Select(t => t.ToDefinition()).ToList() };
            return Ok(response);
        }
        catch ( Exception ex )
        {
            _logger.LogError("Error while fetching user by email: {ErrorMessage}", ex.Message);
            var error = (SaleTicketsError) Enum.Parse(typeof(SaleTicketsError), ex.Message);

            response.Success = false;
            response.Message = error switch
            {
                SaleTicketsError.UserNotFoundWithEmail => SaleTicketsErrorMessage.UserNotFoundWithEmail,
                SaleTicketsError.EmailIsNullOrEmpty => SaleTicketsErrorMessage.EmailIsNullOrEmpty,
                _ => _genericErrorMessage
            };

            return BadRequest(response);
        }
    }

    [HttpPost("register-ticket")]
    [Authorize(Roles = "Admin, Client")]
    public async Task<ActionResult<ServiceResponse<RegisterTicketResponse>>> RegisterTicket(RegisterTicketRequest request)
    {
        var response = new ServiceResponse<RegisterTicketResponse>();
        try
        {
            var ticket = await _saleTicketCommands.RegisterTicket(request);
            response.Success = true;
            response.Message = _successRegisterTicketMessage;
            response.Data = new RegisterTicketResponse { SaleTicket = ticket.ToDefinition() };
            return Ok(response);
        }
        catch ( Exception ex )
        {
            _logger.LogError("Error while registering ticket: {ErrorMessage}", ex.Message);
            var error = (SaleTicketsError) Enum.Parse(typeof(SaleTicketsError), ex.Message);

            response.Success = false;
            response.Message = error switch
            {
                SaleTicketsError.UserNotFoundWithEmail => SaleTicketsErrorMessage.UserNotFoundWithEmail,
                SaleTicketsError.EmailIsNullOrEmpty => SaleTicketsErrorMessage.EmailIsNullOrEmpty,
                SaleTicketsError.FolioIsNullOrEmpty => SaleTicketsErrorMessage.FolioIsNullOrEmpty,
                SaleTicketsError.FolioAlreadyRegistered => SaleTicketsErrorMessage.FolioAlreadyRegistered,
                SaleTicketsError.TimeSpanBetweenTicketRegisterNotReached => SaleTicketsErrorMessage.TimeSpanBetweenTicketRegisterNotReached,
                _ => _genericErrorMessage
            };

            return BadRequest(response);
        }
    }

    [HttpGet("get-total-tickets-count")]
    [Authorize(Roles = "Admin, Client")]
    public async Task<ActionResult<ServiceResponse<GetTotalTicketsCount>>> GetTotalTicketsCount()
    {
        var response = new ServiceResponse<GetTotalTicketsCount>();
        try
        {
            var totalTickets = await _saleTicketCommands.GetTotalTicketsCount();
            response.Success = true;
            response.Message = _successGetTicketsCountMessage;
            response.Data = new GetTotalTicketsCount { TotalTicketsCount = totalTickets };
            return Ok(response);
        }
        catch ( Exception ex )
        {
            _logger.LogError(ex, "Error while fetching tickets count");
            response.Success = false;
            response.Message = _genericErrorMessage;
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }
}
