
namespace GGR.Client;

public static class Routes
{
    public static string Home = "/";
    public static class User
    {
        public static string LoginPage = "/login-admin";
        public static string UserList = "/users";
    }

    public static class Reward
    {
        public static string RewardsList = "/rewards";
        public static string CreateReward = "/rewards/create";
        public static string EditReward = "/rewards/edit";
    }

    public static class RewardClaim
    {
        public static string RewardClaimList = "/reward-claims";
    }

    public static class Tickets
    {
        public static string SaleTicketList = "/sale-tickets";
    }

}
