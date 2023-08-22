namespace GGR.Server.Errors;

public enum SaleTicketsError
{
    UserNotFoundWithEmail,
    EmailIsNullOrEmpty,
    FolioIsNullOrEmpty,
    FolioAlreadyRegistered,
    UserNotRegistered,
    ErrorWhileSavingTicket,
    TimeSpanBetweenTicketRegisterNotReached
}
