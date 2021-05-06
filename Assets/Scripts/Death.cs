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
        foreach(Player player in GameGlobals.GetPlayers()) {
          if (player.playerName != other.gameObject.GetComponentInParent<Player>().playerName) {
            winner.GetComponent<Text>().enabled = true;
            winner.text = player.playerName + " Wins :)";
            MatchManager.manager.EndGame();
          }
        }
      }
    }
}
