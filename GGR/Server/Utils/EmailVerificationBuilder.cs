using GGR.Server.Data.Models;
using static System.Net.WebRequestMethods;

namespace GGR.Server.Utils;

public static class EmailVerificationBuilder
{
    public static string BuildVerificationEmail(User user, string token)
    {
        return $"Para verificar tu cuenta ingresa al siguiente link: http://localhost:5011/verify-user/{token}";
    }
}
