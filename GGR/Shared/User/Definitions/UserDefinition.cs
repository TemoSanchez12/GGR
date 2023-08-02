namespace GGR.Shared.User.Definitions;

public class UserDefinition
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string PhotoUrl { get; set; } = string.Empty;
    public int Points { get; set; }
}
