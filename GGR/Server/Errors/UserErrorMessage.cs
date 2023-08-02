namespace GGR.Server.Errors;

public static class UserErrorMessage
{

    public static string EmailAlreadyRegistered = "El correo electronico ya ha sido registrado a otra cuenta.";
    public static string UnspecifieRole = "Rol de usuario no espesificado.";
    public static string SavingUserError = "Error al gurdar usuario.";
    public static string UserNotFound = "No se ha encontrado el usuario.";
    public static string UserNotVerified = "La cuenta no esta verificada.";
    public static string IncorrectPassword = "El usuario o contraseña no es correcto, por favor verifique los datos.";
    public static string RegistrationExpiredToken = "Token de verification expirado";
    public static string RegistrationNotFound = "Registro no encontrado por favor vuelva a crear cuenta";
}
