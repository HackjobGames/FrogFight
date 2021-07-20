using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatus : MonoBehaviour
{
    public Text[] names;
    public Image[] statuses;
    public Text[] scores;

    public static PlayerStatus status;

    private void Start() {
      status = this;
    }

    public void StartGame() {
      print(GameGlobals.globals.game_mode);
      this.gameObject.SetActive(true);
      Player[] players = GameGlobals.globals.GetPlayers();
      for(int i = 0; i < GameGlobals.globals.GetPlayers().Length; i++) {
        names[i].gameObject.SetActive(true);
        names[i].text = players[i].playerName + ":";
        if (GameGlobals.globals.game_mode == "Survival") {
          statuses[i].gameObject.SetActive(true);
          statuses[i].color = Color.green;
        } else if (GameGlobals.globals.game_mode == "Demolition") {
          scores[i].gameObject.SetActive(true);
          scores[i].text = "0";
        }
      }
    }

    public void UpdateGame() {
      Player[] players = GameGlobals.globals.GetPlayers();
      for(int i = 0; i < GameGlobals.globals.GetPlayers().Length; i++) {
        statuses[i].color = players[i].dead ? Color.red : Color.green;
        scores[i].text = players[i].score + "";
      }
    }

    public void EndGame() {
      this.gameObject.SetActive(false);
      for(int i = 0; i < names.Length; i++) {
        names[i].gameObject.SetActive(false);
        statuses[i].gameObject.SetActive(false);
        scores[i].gameObject.SetActive(false);
      }
    }
}
