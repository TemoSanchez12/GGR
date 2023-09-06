using GGR.Server.Data.Models;

namespace GGR.Server.Utils;

public static class EmailVerificationBuilder
{
    public static string BuildVerificationEmail(string baseUrl, string token)
    {
        return $"Para verificar tu cuenta ingresa al siguiente link: https://{baseUrl}/verify-user/{token}";
    }

    public static string BuildEmailForRestorePassword(string baseUrl, string token)
    {
        return $"Ingrese al siguiente enlace para actualizar la contraseña https://{baseUrl}/reasignar-password/{token}";
    }
}
