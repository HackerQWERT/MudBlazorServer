using Microsoft.AspNetCore.SignalR.Client;
using static System.Net.WebRequestMethods;

namespace MudBlazorServer.Services;

public class SignalR
{
    private static HubConnection aIGalaxyConnection;
    public static HubConnection AIGalaxyConnection
    {
        get => aIGalaxyConnection;
        set => aIGalaxyConnection = value;
    }
    public static string AIGalaxyHub { get; set; } = "AIGalaxyHub";
    
    public static class SignalRMethod
    {

        public static class ServerMethod
        {
            public static string AIChat { get; set; } = "AIChat";
            public static string AIGenerate { get; set; } = "AIGenerate";
            public static string AIVary { get; set; } = "AIVary";
            public static string EditImages { get; set; } = "DALLE2ImagesEdit";
            public static string GenerateInvitationCode { get; set; } = "GenerateInvitationCode";

            public static string BindInvitationCode { get; set; } = "BindInvitationCode";


            public static string RegisterAccount { get; set; } = "RegisterAccount";
            public static string ChangePassword { get; set; } = "ChangePassword";
            public static string Login { get; set; } = "Login";
            public static string InvitationSyncAccountInformation { get; set; } = "InvitationSyncAccountInformation";
            public static string SettingSyncAccountInformation { get; set; } = "SettingSyncAccountInformation";
            public static string PurchaseDrawOrChat { get; set; } = "PurchaseDrawOrChat";
        }
        public static class ClientMethod
        {
            public static string AIChat { get; set; } = "AIChat";

            public static string AIGenerate { get; set; } = "AIGenerate";
            public static string GenerateInvitationCode { get; set; } = "GenerateInvitationCode";
            public static string BindInvitationCode { get; set; } = "BindInvitationCode";
            public static string AIVary { get; set; } = "AIVary";
            public static string EditImages { get; set; } = "EditImages";
            public static string ChangePassword { get; set; } = "ChangePassword";
            public static string Login { get; set; } = "Login";
            public static string RegisterAccount { get; set; } = "RegisterAccount";
            public static string SettingSyncAccountInformation { get; set; } = "SettingSyncAccountInformation";
            public static string InvitationSyncAccountInformation { get; set; } = "InvitationSyncAccountInformation";

        }

    }



}
