using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mirror;
public class Death : NetworkBehaviour
{
    public Text winner;
    string alivePlayerName;

    void OnTriggerEnter(Collider other) {
      Player deadPlayer = other.gameObject.GetComponentInParent<Player>();
      if (deadPlayer == Player.localPlayer && !Player.localPlayer.dead) {
        CmdDeath(deadPlayer);
      }
    }

    [Command (requiresAuthority = false)]
    void CmdDeath(Player deadPlayer) {
      RpcDeath(deadPlayer);
    }

    [ClientRpc]
    void RpcDeath(Player deadPlayer) {
        deadPlayer.dead = true;
        int aliveCount = 0;
        string alivePlayerName = "";
        PlayerStatus.status.UpdateGame();
        Player[] players = GameGlobals.globals.GetPlayers();
        foreach(Player player in players) {
          if (!player.dead) {
            aliveCount++;
            alivePlayerName = player.playerName;
          }
        }
        if (GameGlobals.globals.game_mode == "Survival") {
          if (aliveCount == 1) {
            winner.GetComponent<Text>().enabled = true;
            winner.text = alivePlayerName + " Wins :)";
            MatchManager.manager.EndGame();
          } else if (players.Length == 1) {
            MatchManager.manager.EndGame();
          }
        } else if (GameGlobals.globals.game_mode == "Demolition" && aliveCount == 0) {
          string maxPlayer = "";
          int maxScore = 0;
          foreach(Player player in players) {
            if (player.score > maxScore) {
              maxScore = player.score;
              maxPlayer = player.playerName;
            }
          }
          winner.GetComponent<Text>().enabled = true;
          winner.text = maxPlayer + " Wins :)";
          MatchManager.manager.EndGame();
        }


    }
}
