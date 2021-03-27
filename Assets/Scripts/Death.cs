using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Death : MonoBehaviour
{
    public Text winner;
    void OnCollisionEnter(Collision other) {
      if (other.gameObject.tag == "Player") {
        foreach(Player player in GameGlobals.GetPlayers()) {
          print(other.gameObject.GetComponentInParent<Player>().playerName);
          if (player.playerName != other.gameObject.GetComponentInParent<Player>().playerName) {
            winner.GetComponent<Text>().enabled = true;
            winner.text = player.playerName + " Wins :)";
          }
        }
      }
    }
}
