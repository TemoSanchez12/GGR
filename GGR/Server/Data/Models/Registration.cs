using GGR.Server.Data.Models.Utils;

namespace GGR.Server.Data.Models;

public class Registration
{
    public Guid Id { get; set; }
    public User User { get; set; } = new User();
    public string VerificationToken { get; set; } = string.Empty;
    public string VerificationPhoneCode { get; set; } = string.Empty;
    public DateTime RegistrationDate { get; set; }
    public RegistrationStatus Status { get; set; }
    public DateTime ExpiryTime { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public string? PasswordResetToken { get; set; }
    public DateTime ResetTokenExpires { get; set; }
}
