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
          print(other.gameObject.GetComponentInParent<Player>().playerName);
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
      SceneManager.UnloadScene("Forest");
      Player[] players = GameGlobals.GetPlayers();
      foreach(Player player in players) {
        player.GetComponent<Character>().ResetCharacter();
      }
      MatchManager match = GameObject.FindObjectOfType<MatchManager>();
      match.lobbyCam.SetActive(true);
      match.lobbyUI.SetActive(true);
      match.ChangeMap("");
      Cursor.lockState = CursorLockMode.None;
    }
}
