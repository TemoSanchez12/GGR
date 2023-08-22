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
    public static string EmailIsNullWhenSearching = "No se ha mandado el parametro para hacer la busqueda";
    public static string NotUsersFoundByEmail = "No se han encontrado usuarios con el email proporcionado";
    public static string ErrorSendingVerifycationEmail = "Algo ha salido mal a la hora de enviar email de verificacion";
    public static string UserAlreadyVerified = "El usuario ya esta registrado.";
}
