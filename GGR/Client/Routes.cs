
namespace GGR.Client;

public static class Routes
{
    public const string Dashboard = "/dashboard";

    public static class Customer
    {
        public const string Index = "/";
        public const string LoginCustomer = "/inicio-sesion";
        public const string LoginCustomerSessionExpired = "/inicio-sesion?sessionExpired=true";
        public const string Profile = "/profile";
        public const string ClaimedRewards = "/recompensas-reclamadas";
        public const string RegisterCustomer = "/registrarse";
        public const string ResetPasswordRequest = "/recupear-password";
        public const string RestorePassword = "/reasignar-password/{Token}";
        public const string CustomerRewardList = "/catalogo";
        public const string TermsAndConditions = "/terminos-y-condiciones";
        public const string RegisterTicketNotAvailable = "/mantenimiento";
    }

    public static class User
    {
        public const string LoginPage = "/login";
        public const string UserList = "/users";
        public const string CreateUser = "/register-admin";
        public const string EditUser = "/users/edit/{Id}";
        public const string VerifyUser = "/verify-user/{token}";
        public const string LoginPageSesionExpired = "/login-admin?sessionExpired=true";
    }

    public static class Reward
    {
        public const string RewardsList = "/rewards";
        public const string CreateReward = "/rewards/create";
        public const string EditReward = "/rewards/edit/{Id}";
    }

    public static class RewardClaim
    {
        public const string RewardClaimList = "/reward-claims";
    }

    public static class Tickets
    {
        public const string SaleTicketList = "/sale-tickets";
    }

    public static class FileRecord
    {
        public const string ManageFileRecords = "/manage-file-records";
    }

    public static string ParseId(string route, string id)
    {
        return route.Replace("{Id}", id);
    }
}
