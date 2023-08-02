using Discord;
using System.Data;
using System;
using System.Runtime.Serialization;
using Discord.WebSocket;

[DataContract]
public class REGISTRATIONBUTTON : BaseButton
{
    public REGISTRATIONBUTTON()
    {
        buttonName = ButtonName.REGISTRATIONBUTTON;
        thisInterfaceButton.ButtonLabel = "REGISTER";
        buttonStyle = ButtonStyle.Primary;
        ephemeralResponse = true;
    }

    protected override string GenerateCustomButtonProperties(int _buttonIndex, ulong _leagueCategoryId)
    {
        return "";
    }

    public override Task<Response> ActivateButtonFunction(
        SocketMessageComponent _component, InterfaceMessage _interfaceMessage)
    {
        Log.WriteLine("Starting player registration", LogLevel.DEBUG);
        InterfaceChannel registrationChannel;

        string registrationChannelCheck;

        try
        {
            registrationChannel = Database.Instance.Categories.FindInterfaceCategoryWithCategoryId(
                _interfaceMessage.MessageCategoryId).FindInterfaceChannelWithNameInTheCategory(
                    ChannelType.LEAGUEREGISTRATION);
            registrationChannelCheck = "\n\n Check <#" + registrationChannel.ChannelId + "> for all the available leagues!";
        }
        catch (Exception ex)
        {
            Log.WriteLine(ex.Message, LogLevel.ERROR);
            return Task.FromResult(new Response(ex.Message, false));
        }

        // Checks that the player does not exist in the database already, true if this is not the case
        if (Database.Instance.PlayerData.AddNewPlayerToTheDatabaseById(_component.User.Id).Result)
        {
            Database.Instance.CachedUsers.AddUserIdToCachedConcurrentBag(_component.User.Id);
            return Task.FromResult(new Response(_component.User.Mention + ", " +
                BotMessaging.GetMessageResponse(
                    _component.Data.CustomId,
                    " registration complete, welcome!" + registrationChannelCheck,
                    _component.Channel.Name), false));

        }
        else
        {
            return Task.FromResult(new Response(_component.User.Mention + ", " +
                BotMessaging.GetMessageResponse(
                    _component.Data.CustomId,
                    " You are already registered!" + registrationChannelCheck,
                    _component.Channel.Name), false));
        }
    }
}