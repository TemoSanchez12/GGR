using System.Net.NetworkInformation;

namespace GGR.Client;

public static class Routes
{
    public static string Home = "/";
    public static class User
    {
        public static string LoginPage = "/login-admin";
    }

    public static class Reward
    {
        public static string RewardsList = "/rewards";
        public static string CreateReward = "/rewards/create";
        public static string EditReward = "/rewards/edit";
    }

}
