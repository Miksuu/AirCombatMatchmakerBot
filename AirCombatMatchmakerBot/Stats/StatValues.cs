using System.Runtime.Serialization;

[DataContract]
public class StatValues : logClass<StatValues>
{
    public int Wins
    {
        get => wins.GetValue();
        set => wins.SetValue(value);
    }

    public int Losses
    {
        get => losses.GetValue();
        set => losses.SetValue(value);
    }

    public int Kills
    {
        get => kills.GetValue();
        set => kills.SetValue(value);
    }

    public int Deaths
    {
        get => deaths.GetValue();
        set => deaths.SetValue(value);
    }

    public int Streak
    {
        get => streak.GetValue();
        set => streak.SetValue(value);
    }

    public float WinLoseRatio
    {
        get => winLoseRatio.GetValue();
        set => winLoseRatio.SetValue(value);
    }

    public float KillDeathRatio
    {
        get => killDeathRatio.GetValue();
        set => killDeathRatio.SetValue(value);
    }

    // Add plane specific stats to see how the player performs against each of the enemy planes

    [DataMember] private logVar<int> wins = new logVar<int>();
    [DataMember] private logVar<int> losses = new logVar<int>();
    [DataMember] private logVar<int> kills = new logVar<int>();
    [DataMember] private logVar<int> deaths = new logVar<int>();
    [DataMember] private logVar<int> streak = new logVar<int>();

    private logVar<float> winLoseRatio = new logVar<float>();
    private logVar<float> killDeathRatio = new logVar<float>();

    public StatValues() 
    {
        CalculateFloats();
    }

    public void CalculateFloats()
    {
        if (Losses != 0)
        {
            WinLoseRatio = (float)Wins / (Wins + Losses);
        }
        else
        {
            if (Wins != 0)
            {
                WinLoseRatio = 1f;
            }
            else
            {
                WinLoseRatio = 0;
            }
        }

        WinLoseRatio *= 100;

        if (Deaths != 0)
        {
            KillDeathRatio = (float)Kills / Deaths;
        }
        else
        {
            if (Kills != 0)
            {
                KillDeathRatio = (float)Kills;
            }
            else
            {
                KillDeathRatio = 0;
            }
        }
    }
}