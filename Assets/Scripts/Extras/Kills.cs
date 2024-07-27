using System;

public class Kills : IComparable<Kills> {
    public string playerName;
    public int playerKills;

    public Kills(string t_newPlayerName, int t_newPlayerScore) {
        playerName = t_newPlayerName;
        playerKills = t_newPlayerScore;
    }
    public int CompareTo(Kills t_other) {
        return t_other.playerKills - playerKills;
    }
}