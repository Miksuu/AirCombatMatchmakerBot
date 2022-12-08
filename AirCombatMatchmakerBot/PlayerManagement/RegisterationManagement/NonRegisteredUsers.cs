[Serializable]
public class NonRegisteredUser
{
    public ulong discordUserId { get; set; }
    public ulong discordRegisterationChannelId { get; set; }

    NonRegisteredUser() { }
    public NonRegisteredUser(ulong discordUserId)
    {
        this.discordUserId = discordUserId;
    }

    public string ConstructChannelName()
    {
        return "registeration_" + discordUserId;
    }
}