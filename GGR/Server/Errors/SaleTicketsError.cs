namespace GGR.Server.Errors;

public enum SaleTicketsError
{
    UserNotFoundWithEmail,
    UserNotFoundWithId,
    EmailIsNullOrEmpty,
    IdIsNullOrEmpty,
    FolioIsNullOrEmpty,
    FolioAlreadyRegistered,
    UserNotRegistered,
    ErrorWhileSavingTicket,
    TimeSpanBetweenTicketRegisterNotReached
}
