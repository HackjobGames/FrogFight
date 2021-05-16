public class Match
{
    public string HostName;
    public string MatchID;
    public int RelayID;
    public bool Private;
    public string Password;
    public int MaxPlayers;
    public int CurrentPlayers;

    public Match(string MatchID, int RelayID, bool Private, string Password, int MaxPlayers, int CurrentPlayers) {
        this.MatchID = MatchID;
        this.RelayID = RelayID;
        this.Private = Private;
        this.Password = Password;
        this.MaxPlayers = MaxPlayers;
        this.CurrentPlayers = CurrentPlayers;
    }
}
