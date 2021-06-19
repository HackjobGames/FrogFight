using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class Death : MonoBehaviour
{
    public Text winner;
    bool finished = false;
    void OnCollisionEnter(Collision other) {
      if (other.gameObject.tag == "Player" && !finished) {
        finished = true;
        other.gameObject.GetComponentInParent<Player>().dead = true;
        int aliveCount = 0;
        string alivePlayerName = "";
        Player[] players = GameGlobals.globals.GetPlayers();
        foreach(Player player in players) {
          if (!player.dead) {
            aliveCount++;
            alivePlayerName = player.playerName;
          }
        }
        if (aliveCount == 1) {
          winner.GetComponent<Text>().enabled = true;
          winner.text = alivePlayerName + " Wins :)";
          MatchManager.manager.EndGame();
        } else if (players.Length == 1) {
          MatchManager.manager.EndGame();
        }
      }
    }
}
