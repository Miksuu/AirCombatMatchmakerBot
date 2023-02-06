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
        buttonLabel = "REGISTER";
        buttonStyle = ButtonStyle.Primary;
    }

    public void CreateTheButton(){}

    public override Task<(string, bool)> ActivateButtonFunction(
        SocketMessageComponent _component, InterfaceMessage _interfaceMessage)
    {
        Log.WriteLine("Starting player registration", LogLevel.DEBUG);

        string response = string.Empty;
        string registrationChannelCheck = string.Empty;
        bool serialize = true;

        var registrationChannel = Database.Instance.Categories.FindCreatedCategoryWithChannelKvpWithId(
            _interfaceMessage.MessageCategoryId).Value.FindInterfaceChannelWithNameInTheCategory(
        ChannelType.LEAGUEREGISTRATION);

        if (registrationChannel == null)
        {
            Log.WriteLine(nameof(registrationChannel) + " was null", LogLevel.ERROR);
            registrationChannelCheck = nameof(registrationChannel) + " was null!";
        }
        else
        {
            registrationChannelCheck = "\n\n Check <#" + registrationChannel.ChannelId + "> for all the available leagues!";
        }

        // Checks that the player does not exist in the database already, true if this is not the case
        if (Database.Instance.PlayerData.AddNewPlayerToTheDatabaseById(_component.User.Id).Result)
        {
            Database.Instance.CachedUsers.AddUserIdToCachedList(_component.User.Id);

            response = _component.User.Mention + ", " +
                BotMessaging.GetMessageResponse(
                    _component.Data.CustomId,
                    " registration complete, welcome!" + registrationChannelCheck,
                    _component.Channel.Name);
        }
        else
        {
            serialize = false;
            response = _component.User.Mention + ", " +
                BotMessaging.GetMessageResponse(
                    _component.Data.CustomId,
                    " You are already registered!" + registrationChannelCheck,
                    _component.Channel.Name);
        }

        return Task.FromResult((response, serialize));
    }
}