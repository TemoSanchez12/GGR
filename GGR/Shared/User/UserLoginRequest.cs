

using System.ComponentModel.DataAnnotations;

namespace GGR.Shared.User;

public class UserLoginRequest
{
    [Required(ErrorMessage = "Por favor ingrese el correo electronico")]
    [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Debes indicar un email válido")]
    [MaxLength(100, ErrorMessage = "El correo electronico no puede tener más de 100 caracteres")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tiene que ingresar la contraseña")]
    [MinLength(8, ErrorMessage = "La contraseña tiene que tener más de 8 caracteres")]
    [MaxLength(30, ErrorMessage = "La contraseña no puede tener más de 30 caracteres")]
    public string Password { get; set; } = string.Empty;
}
