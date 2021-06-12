using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class GameGlobals : NetworkBehaviour
{
    [SyncVar]
    public float max_tongue_distance = 150f;
    [SyncVar]
    public float slam_power = 20000f;

    public InputField tongue_input;
    public InputField slam_input;

    public Text[] playerNames;

    public static GameGlobals globals;

    private void Start() {
      globals = this;
    }

    public Player[] GetPlayers() {
      GameObject[] prefabs = GameObject.FindGameObjectsWithTag("PlayerRoot");
      Player[] players = new Player[prefabs.Length];
      for (int i = 0; i < playerNames.Length; i++) {
        if (i < players.Length) {
          players[i] = prefabs[i].GetComponent<Player>();
          playerNames[i].text = players[i].playerName;
        } else {
          playerNames[i].text = "";
        }
        
      }
      return players;
    }



    public void UpdateTongueDistance () {
      max_tongue_distance = float.Parse(tongue_input.text);
    } 
    
    public void UpdateSlamPower() {
      slam_power = float.Parse(slam_input.text);
    }
}
