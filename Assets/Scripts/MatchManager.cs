using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using UnityEngine.UI;
using UnityEngine.Networking;

public class MatchManager : NetworkBehaviour
{
  [SyncVar]
  public string map;
  public Image forest;
  public Image destructibleTest;
  public Image checkMark;
  public Button playButton;
  public bool inGame = false;
  public static MatchManager manager;

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
    manager = this;
    if (this.isServer) {
      forest.GetComponent<Button>().interactable = true;
      checkMark.GetComponent<Button>().interactable = true;
    }
  }
  [ClientRpc]
  public void LoadMap() {
    SceneManager.LoadScene(map, LoadSceneMode.Additive);
    inGame = true;
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
    Player[] players = GameGlobals.globals.GetPlayers();
    CmdSetLoadedFlag(Player.localPlayer.playerName, players);
    yield return new WaitUntil(() => {
      foreach(Player player in players) {
        if (!player.loaded) {
          return false;
        }
      }
      return true;
    });
    ServerManager.server.lobbyUI.GetComponent<Canvas>().enabled = false;
    MainMenu.menu.mainMenuUi.SetActive(false);
    MainMenu.menu.menuCamera.SetActive(false);
    GameObject[] spawns = GameObject.FindGameObjectsWithTag("SpawnPosition");
    for (int i = 0; i < players.Length; i++) {
      players[i].GetComponent<Character>().enabled = true;
      players[i].GetComponentInChildren<Impact>().transform.position = spawns[i].transform.position;
    }
  }

  public void EndGame() {
    StartCoroutine(DelayEndGame());
  }

  public IEnumerator DelayEndGame() {
    yield return new WaitForSeconds(3);
    MatchManager match = GameObject.FindObjectOfType<MatchManager>();
    Player[] players = GameGlobals.globals.GetPlayers();
    foreach(Player player in players) {
      player.GetComponent<Character>().ResetCharacter();
      player.dead = false;
    }
    ServerManager.server.lobbyUI.GetComponent<Canvas>().enabled = true;
    MainMenu.menu.menuCamera.SetActive(true);
    SceneManager.UnloadScene(match.map);
    match.ChangeMap("");
    Cursor.lockState = CursorLockMode.None;
    inGame = false;
  }

  override public void OnStopClient() {
    Destroy(ServerManager.server.lobbyUI);
    MainMenu.menu.mainMenuUi.SetActive(true);
    MainMenu.menu.menuCamera.SetActive(true);
    Cursor.lockState = CursorLockMode.None;
    string route = this.isServer ? "Match" : "Player"; 
    UnityWebRequest req = UnityWebRequest.Get($"http://localhost:8090/remove{route}?matchID={ServerManager.server.matchID}");
    req.SendWebRequest();
  }
}
