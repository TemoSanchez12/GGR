
using System.ComponentModel.DataAnnotations;

namespace GGR.Shared.User;

public class UserRegisterRequest
{
    [Required(ErrorMessage = "Este campo es requerido")]
    [MaxLength(100, ErrorMessage = "El nombre no puede tener más 100 caracteres")]
    [MinLength(5, ErrorMessage = "El nombre tiene que tener minimo 5 caracteres")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Este campo es requerido")]
    [MaxLength(100, ErrorMessage = "Los apellidos no pueden tener más 100 caracteres")]
    [MinLength(5, ErrorMessage = "Los apellidos tienen que tener minimo 5 caracteres")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Por favor ingrese el correo electronico")]
    [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Debes indicar un email válido")]
    [MaxLength(100, ErrorMessage = "El correo electronico no puede tener más de 100 caracteres")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Este campo es requerido")]
    [MaxLength(10, ErrorMessage = "El numero telefonico tiene que tener 10 digitos")]
    [MinLength(10, ErrorMessage = "El numero telefonico tiene que tener 10 digitos")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Este campo es requerido")]
    [MinLength(8, ErrorMessage = "La contraseña tiene que tener minimo 8 caracteres")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Este campo es requerido"), Compare("Password", ErrorMessage = "Las dos contraseñas tienen que conicidir")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required]
    public string UserRol { get; set; } = string.Empty;
}
