
using System.ComponentModel.DataAnnotations;

namespace GGR.Shared.User;

public class UserRegisterRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(10), MaxLength(10)]
    public string Phone { get; set; } = string.Empty;

    [Required, MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required, Compare("Password")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required]
    public string UserRol { get; set; } = string.Empty;
}
