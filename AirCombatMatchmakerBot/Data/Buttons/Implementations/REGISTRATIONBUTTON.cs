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

    public override async Task<string> ActivateButtonFunction(
        SocketMessageComponent _component, InterfaceMessage _interfaceMessage)
    {
        Log.WriteLine("Starting player registration", LogLevel.DEBUG);

        string response = string.Empty;
        // Checks that the player does not exist in the database already, true if this is not the case
        if (Database.Instance.PlayerData.AddNewPlayerToTheDatabaseById(_component.User.Id).Result)
        {
            Database.Instance.CachedUsers.AddUserIdToCachedList(_component.User.Id);

            response = _component.User.Mention + ", " +
                BotMessaging.GetMessageResponse(
                    _component.Data.CustomId,
                    " registration complete, welcome!",
                    _component.Channel.Name);
        }
        else
        {
            response = _component.User.Mention + ", " +
                BotMessaging.GetMessageResponse(
                    _component.Data.CustomId,
                    " You are already registered!",
                    _component.Channel.Name);
        }

        return Task.FromResult(response).Result;
    }
}