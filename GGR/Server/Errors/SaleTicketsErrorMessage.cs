namespace GGR.Server.Errors;

public static class SaleTicketsErrorMessage
{
    public static string UserNotFoundWithEmail = "No se ha encontrado ningun usuario con el email proporcionado";
    public static string EmailIsNullOrEmpty = "No se ha proporcionado ningún email";
    public static string FolioIsNullOrEmpty = "No se ha proporcionado ningún folio";
    public static string FolioAlreadyRegistered = "El folio proporcionado ya ha sido registrado";
    public static string TimeSpanBetweenTicketRegisterNotReached = "Tiene que esperar 30 minutos para registrar otro ticket";
}
