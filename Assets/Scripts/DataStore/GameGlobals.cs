using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class GameGlobals : NetworkBehaviour
{
    public Text p1Text;
    public Text p2Text;
    [SyncVar]
    public float max_tongue_distance = 150f;
    [SyncVar]
    public float slam_power = 1000f;

    public InputField tongue_input;
    public InputField slam_input;
  
    private void Update() {
      Player[] players = GetPlayers();
      if (players.Length > 0) {
        p1Text.text = "Player Name: " + players[0].playerName;
      }
      if (players.Length > 1) {
        p2Text.text = "Player Name: " + players[1].playerName;
      }
    }
    public static Player[] GetPlayers() {
      GameObject[] prefabs = GameObject.FindGameObjectsWithTag("PlayerRoot");
      Player[] players = new Player[prefabs.Length];
      for (int i = 0; i < prefabs.Length; i++) {
        players[i] = prefabs[i].GetComponent<Player>();
      }
      return players;
    }

    public void UpdateTongueDistance (string value) {
      max_tongue_distance = float.Parse(tongue_input.text);
    } 
    
    public void UpdateSlamPower(string value) {
      slam_power = float.Parse(slam_input.text);
    }
}
