using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class GameGlobals : NetworkBehaviour
{
    public Text p1Text;
    public Text p2Text;
    public static bool levelLoaded = false;
  
    private void Update() {
      Player[] players = GetPlayers();
      if (players.Length > 0) {
        p1Text.text = "Player Name: " + players[0].name;
      }
      if (players.Length > 1) {
        p2Text.text = "Player Name: " + players[1].name;
      }
    }
    public static Player[] GetPlayers() {
      GameObject[] prefabs = GameObject.FindGameObjectsWithTag("Player");
      Player[] players = new Player[prefabs.Length];
      for (int i = 0; i < prefabs.Length; i++) {
        players[i] = prefabs[i].GetComponent<Player>();
      }
      return players;
    }
}
