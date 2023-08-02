using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GGR.Shared.User;

public class ResetPasswordRequest
{
    [Required]
    public string ResetToken { get; set; } = string.Empty;

    [Required, MinLength(6)]
    public string NewPassword { get; set; } = string.Empty;

    [Required, Compare("NewPassword")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
