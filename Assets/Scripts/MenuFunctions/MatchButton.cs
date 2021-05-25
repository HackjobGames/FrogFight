using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchButton : MonoBehaviour
{
    public Match match;
    public Text isPrivate;
    public Text hostName;
    public Text playerCount;



    public void SetMatch(Match match) {
      this.match = match;
      isPrivate.text = match.Private ? "Yes" : "No";
      hostName.text = match.HostName;
      playerCount.text = match.CurrentPlayers + "/" + match.MaxPlayers;
    }

    public void JoinMatch() {
        MainMenu.menu.JoinFromMatch(match);
    }


}
