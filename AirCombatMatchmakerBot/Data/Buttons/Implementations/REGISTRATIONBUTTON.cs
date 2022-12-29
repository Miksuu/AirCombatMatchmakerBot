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

    public void CreateTheButton()
    {

    }

    public void ActivateButtonFunction()
    {

    }
}