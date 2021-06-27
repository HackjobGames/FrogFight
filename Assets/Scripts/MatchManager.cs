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
  [SyncVar]
  public bool serverLoaded = false;

  public Image forest;
  public Image checkMark;
  public Button playButton;
  public bool inGame = false;
  public static MatchManager manager;
  public Material[] playerMaterials = new Material[4];
  public Image[] playerColors = new Image[4];
  public GameObject forestPrefab;
  public GameObject currentMapInstance;

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
      }
    }
  }

  private void Start() {
    manager = this;
    playButton.interactable = false;
    if (this.isServer) {
      forest.GetComponent<Button>().interactable = true;
      checkMark.GetComponent<Button>().interactable = true;
    }
    for(int i = 0; i < playerColors.Length; i++){
      playerColors[i].color = playerMaterials[i].color;
    }
  }
  
  [ClientRpc]
  public void LoadMap() {
    inGame = true;
    playButton.interactable = false;
    StartCoroutine(WaitForMap());
  }




  IEnumerator WaitForMap() {
    if (this.isServer) {
      currentMapInstance = ServerManager.server.SpawnNetworkObject(forestPrefab);
      serverLoaded = true;
    }
    yield return new WaitUntil(() => serverLoaded);
    Player[] players = GameGlobals.globals.GetPlayers();
    ServerManager.server.lobbyUI.GetComponent<Canvas>().enabled = false;
    MainMenu.menu.mainMenuUi.SetActive(false);
    MainMenu.menu.menuCamera.SetActive(false);
    GameObject[] spawns = GameObject.FindGameObjectsWithTag("SpawnPosition");
    for (int i = 0; i < players.Length; i++) {
      players[i].GetComponent<Character>().enabled = true;
      players[i].dead = false;
      players[i].GetComponentInChildren<Impact>().transform.position = spawns[i].transform.position;
      players[i].GetComponentInChildren<SkinnedMeshRenderer>().material = playerMaterials[i];
    }
    CmdSetLoadedFlag(Player.localPlayer);
    yield return new WaitUntil(() => {
      foreach(Player player in players) {
        if (!player.loaded) {
          return false;
        }
      }
      return true;
    });
  }

  [Command (requiresAuthority = false)]
  void CmdSetLoadedFlag(Player player) {
    player.loaded = true;
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
    print(gObject);
    Destroy(gObject);
  }

  public void EndGame() {
    StartCoroutine(DelayEndGame());
  }

  public IEnumerator DelayEndGame() {
    yield return new WaitForSeconds(3);
    if (this.isServer) {
      UnityWebRequest req = UnityWebRequest.Get($"http://66.41.159.125:8090/ping?matchID={ServerManager.server.matchID}");
      req.SendWebRequest();
    }
    Player[] players = GameGlobals.globals.GetPlayers();
    foreach(Player player in players) {
      player.GetComponent<Character>().ResetCharacter();
      player.dead = true;
    }
    ServerManager.server.lobbyUI.GetComponent<Canvas>().enabled = true;
    MainMenu.menu.menuCamera.SetActive(true);
    MatchManager.manager.ChangeMap("");
    Cursor.lockState = CursorLockMode.None;
    inGame = false;
    if (this.isServer) {
      playButton.interactable = true;
      ServerManager.server.DestroyNetworkObject(currentMapInstance);
      serverLoaded = false;
    }
  }

  override public void OnStopClient() {
    base.OnStopClient();
    Destroy(ServerManager.server.lobbyUI);
    MainMenu.menu.mainMenuUi.SetActive(true);
    MainMenu.menu.menuCamera.SetActive(true);
    if (MatchManager.manager.inGame) {
      SceneManager.UnloadScene(MatchManager.manager.map);
    }
    Cursor.lockState = CursorLockMode.None;
    string route = this.isServer ? "Match" : "Player"; 
    UnityWebRequest req = UnityWebRequest.Get($"http://66.41.159.125:8090/remove{route}?matchID={ServerManager.server.matchID}");
    req.SendWebRequest();
  }
}
