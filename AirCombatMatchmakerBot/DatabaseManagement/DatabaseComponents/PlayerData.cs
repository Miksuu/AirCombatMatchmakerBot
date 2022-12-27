using System.Runtime.Serialization;

[DataContract]
public class PlayerData
{
    [DataMember] public Dictionary<ulong, Player> PlayerIDs { get; set; }
    public PlayerData()
    {
        PlayerIDs = new Dictionary<ulong, Player>();
    }
}