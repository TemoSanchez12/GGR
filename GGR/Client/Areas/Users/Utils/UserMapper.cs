using GGR.Client.Areas.Users.Models;
using GGR.Shared.User.Definitions;

namespace GGR.Client.Areas.Users.Utils;

public static class UserMapper
{
    public static User MapToEntity(UserDefinition definition)
    {
        return new User
        {
            Id = definition.Id,
            Name = definition.Name,
            LastName = definition.LastName,
            Email = definition.Email,
            Points = definition.Points,
            Phone = definition.Phone
        };
    }
}
