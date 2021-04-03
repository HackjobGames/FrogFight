using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using UnityEngine.UI;

public class MatchManager : NetworkBehaviour
{
  [SyncVar]
  public string map;

  public GameObject lobbyUI;
  public GameObject lobbyCam;
  public Image forest;
  public Image destructibleTest;
  public Image checkMark;
  public Button playButton;

  public void ChangeMap(string map) {
    if (map == "") {
      map = null;
      playButton.interactable = false;
      checkMark.enabled = false;
    } else {
      this.map = map;
      checkMark.enabled = true;
      playButton.interactable = true;
      if (map == "Forest") {
        checkMark.rectTransform.position = forest.rectTransform.position;
      } else if (map == "DestructibleTest") {
        checkMark.rectTransform.position = destructibleTest.rectTransform.position;
      }
    }
  }

  private void Start() {
    if (this.isServer) {
      forest.GetComponent<Button>().interactable = true;
      checkMark.GetComponent<Button>().interactable = true;
    }
  }
  [ClientRpc]
  public void LoadMap() {
    lobbyUI.SetActive(false);
    lobbyCam.SetActive(false);
    SceneManager.LoadScene(map, LoadSceneMode.Additive);
    StartCoroutine(AfterLoad());
  }

  public static void EndMatch() {
    SceneManager.LoadScene("MainMenu");
  }

  IEnumerator AfterLoad() {
    yield return new WaitUntil(() => GameGlobals.levelLoaded);
    Player[] players = GameGlobals.GetPlayers();
    GameObject[] spawns = GameObject.FindGameObjectsWithTag("SpawnPosition");
    for (int i = 0; i < players.Length; i++) {
      players[i].GetComponent<Character>().enabled = true;
      players[i].GetComponentInChildren<Impact>().transform.position = spawns[i].transform.position;
    }
  }
}
