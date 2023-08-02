using GGR.Server.Data.Models.Utils;
using GGR.Shared.User.Definitions;

namespace GGR.Server.Data.Models;

public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string PhotoUrl { get; set; } = string.Empty;
    public byte[] PasswordHash { get; set; } = new byte[32];
    public byte[] PasswordSalt { get; set; } = new byte[32];
    public UserRole Rol { get; set; }
    public int Points { get; set; }

    public UserDefinition ToDefinition()
    {
        return new UserDefinition()
        {
            Id = this.Id,
            Name = this.Name,
            LastName = this.LastName,
            Email = this.Email,
            Phone = this.Phone,
            PhotoUrl = this.PhotoUrl,
            Points = this.Points,
        };
    }
}
