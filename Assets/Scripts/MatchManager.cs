using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using UnityEngine.UI;

public class MatchManager : NetworkBehaviour
{
  [SyncVar(hook=nameof(LoadMap))]
  public string map;

  public GameObject canvas;
  public GameObject lobbyCam;
  public Button playButton;

  public void ChangeMap(string map) {
    this.map = map;
  }

  private void Start() {
    if (this.isServer) {
      playButton.interactable = true;
    }
  }

  public void LoadMap(string oldMap, string newMap) {
    canvas.SetActive(false);
    lobbyCam.SetActive(false);
    SceneManager.LoadScene(newMap, LoadSceneMode.Additive);
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
      players[i].transform.position = spawns[i].transform.position;
    }
  }
}
