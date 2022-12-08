[Serializable]
public class PlayerData
{
    public Dictionary<ulong, Player> PlayerIDs { get; set; }
    public PlayerData()
    {
        PlayerIDs = new Dictionary<ulong, Player>();
    }
}