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
            StartCoroutine(EndGame());
          }
        }
      }
    }

    IEnumerator EndGame() {
      yield return new WaitForSeconds(3);
      MatchManager match = GameObject.FindObjectOfType<MatchManager>();
      Player[] players = GameGlobals.GetPlayers();
      foreach(Player player in players) {
        player.GetComponent<Character>().ResetCharacter();
      }
      ServerManager.lobbyUI.SetActive(true);
      ServerManager.lobbyCamera.SetActive(true);
      GameObject.Find("MainMenuUI").SetActive(true);
      SceneManager.UnloadScene(match.map);
      match.ChangeMap("");
      Cursor.lockState = CursorLockMode.None;
    }
}
