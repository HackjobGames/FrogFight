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
    SceneManager.LoadScene(map, LoadSceneMode.Additive);
    StartCoroutine(AfterLoad());
  }

  [Command (requiresAuthority = false)]
  void CmdSetLoadedFlag(string playerName, Player[] players) {
    foreach(Player player in players) {
      print(player.playerName);
      print(playerName);
      if (player.playerName == playerName) {
        player.loaded = true;
      }
    }
  }

  public static void EndMatch() {
    SceneManager.LoadScene("MainMenu");
  }

  [Command (requiresAuthority = false)]
  public void DestroyObject (GameObject gObject) {
    DestroyObjectClient(gObject);
  }
  [ClientRpc]
  public void DestroyObjectClient(GameObject gObject) {
    Destroy(gObject);
  }

  IEnumerator AfterLoad() {
    Player[] players = GameGlobals.GetPlayers();
    CmdSetLoadedFlag(Player.localPlayer.playerName, players);
    yield return new WaitUntil(() => {
      foreach(Player player in players) {
        if (!player.loaded) {
          return false;
        }
      }
      return true;
    });
    lobbyUI.SetActive(false);
    lobbyCam.SetActive(false);
    GameObject[] spawns = GameObject.FindGameObjectsWithTag("SpawnPosition");
    for (int i = 0; i < players.Length; i++) {
      players[i].GetComponent<Character>().enabled = true;
      players[i].GetComponentInChildren<Impact>().transform.position = spawns[i].transform.position;
    }
  }
}
