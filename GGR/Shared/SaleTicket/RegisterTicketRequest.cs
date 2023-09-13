using System.ComponentModel.DataAnnotations;

namespace GGR.Shared.SaleTicket;

public class RegisterTicketRequest
{
    public string UserId { get; set; } = string.Empty;
    public string Folio { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Este campo es requerido")]
    [Range(1, 12, ErrorMessage = "Ingrese una hora valida, entre 1 y 12")]
    public int Hour { get; set; } = 0;

    [Required(ErrorMessage = "Este campo es requerido")]
    [Range(0, 59, ErrorMessage = "Ingrese un minuto valido, entre 0 y 59")]
    public int Minutes { get; set; } = 0;

    [Required(ErrorMessage = "Este campo es requerido")]
    public string NoonOrMorning { get; set; } = "PM";
}
