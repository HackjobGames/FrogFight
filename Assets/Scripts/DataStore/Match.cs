using Newtonsoft.Json;

public class Match
{
    public string MatchID;
    public string HostName;
    public bool Private;
    public int MaxPlayers;
    public int CurrentPlayers;

    public Match(string MatchID, string HostName, bool Private, int MaxPlayers, int CurrentPlayers) {
        this.MatchID = MatchID;
        this.HostName = HostName;
        this.Private = Private;
        this.MaxPlayers = MaxPlayers;
        this.CurrentPlayers = CurrentPlayers;
    }
}
