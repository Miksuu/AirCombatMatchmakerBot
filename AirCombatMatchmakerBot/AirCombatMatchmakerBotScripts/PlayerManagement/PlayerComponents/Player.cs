﻿using System.Runtime.Serialization;

[DataContract]
public class Player
{   
    public ulong PlayerDiscordId
    {
        get => playerDiscordId.GetValue();
        set => playerDiscordId.SetValue(value);
    }

    public string PlayerNickName
    {
        get => playerNickName.GetValue();
        set => playerNickName.SetValue(value);
    }

    [DataMember] private logVar<ulong> playerDiscordId = new logVar<ulong>();
    [DataMember] private logString playerNickName = new logString();
    [DataMember] public Stats PlayerStats = new Stats();

    public Player()
    {
        PlayerDiscordId = 0;
        PlayerNickName = string.Empty;
    }

    public Player(ulong _playerDiscordID, string _playerNickName)
    {
        PlayerDiscordId = _playerDiscordID;
        PlayerNickName = _playerNickName;
    }

    public string GetPlayerIdAsMention()
    {
        Log.WriteLine("Getting player: " + PlayerNickName + " (" +
            PlayerDiscordId + ") as a mention");

        return "<@" + PlayerDiscordId.ToString() + ">";
    }
}