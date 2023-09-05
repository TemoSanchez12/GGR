using GGR.Server.Data.Models;

namespace GGR.Server.Utils;

public static class EmailVerificationBuilder
{
    public static string BuildVerificationEmail(User user, string token)
    {
        return $"Para verificar tu cuenta ingresa al siguiente link: https://localhost:7069/verify-user/{token}";
    }

    public static string BuildEmailForRestorePassword(string token)
    {
        return $"Ingrese al siguiente enlace para actualizar la contraseña https://localhost:7069/reasignar-password/{token}";
    }
}
