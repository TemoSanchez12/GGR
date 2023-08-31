namespace GGR.Server.Errors;

public enum RewardClaimError
{
    UserNotFound,
    UserNotVerified,
    RewardNotFound,
    NotEnoughPoints,
    ErrorSavingData,
    RewardClaimNotFound,
    NoAllowToAssignStatus,
    IdIsNullOrEmpty,

}
