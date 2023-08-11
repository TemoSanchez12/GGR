
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
}
