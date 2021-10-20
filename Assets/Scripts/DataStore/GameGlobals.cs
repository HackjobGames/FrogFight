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
    [SyncVar]
    public string game_mode;

    public Dropdown game_mode_dropdown;

    public InputField tongue_input;
    public InputField slam_input;

    public Text[] playerNames;
    public Image[] playerColors = new Image[4];

    public static GameGlobals globals;

    private void Start() {
      globals = this;
      SetDefaultValues();
    }

    void SetDefaultValues() {
      slam_input.text = slam_power + "";
      tongue_input.text = max_tongue_distance + "";
      game_mode = "Survival";
    }

    public void UpdateGameMode() {
      game_mode = game_mode_dropdown.captionText.text;
    }

    public Player[] GetPlayers() {
      GameObject[] prefabs = GameObject.FindGameObjectsWithTag("PlayerRoot");
      Player[] players = new Player[prefabs.Length];
      for (int i = 0; i < playerNames.Length; i++) {
        if (i < players.Length) {
          players[i] = prefabs[i].GetComponent<Player>();
          if (playerNames[i]) {
            playerNames[i].text = players[i].playerName;
          }
          playerColors[i].color = players[i].color;
        } else {
          if (playerNames[i]) {
            playerNames[i].text = "";
          }
          playerColors[i].color = new Color(0,0,0,0);
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
