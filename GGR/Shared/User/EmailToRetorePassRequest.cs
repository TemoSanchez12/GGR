
using System.ComponentModel.DataAnnotations;

namespace GGR.Shared.User;

public class EmailToRetorePassRequest
{
    [Required(ErrorMessage = "Este campo es obligatorio")]
    [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Debes indicar un email válido")]
    [MaxLength(100, ErrorMessage = "El correo electronico no puede tener más de 100 caracteres")]
    public string Email { get; set; } = string.Empty;
}
