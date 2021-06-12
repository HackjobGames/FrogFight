using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Player : NetworkBehaviour
{
  [SyncVar]
  public string playerName = null;

  [SyncVar]
  public bool dead = false;

  [SyncVar]
  public bool loaded;

  public static Player localPlayer;
  
  public void Start() {
    StartCoroutine(WaitForGlobals());
    if (this.isLocalPlayer) {
      SetName(MainMenu.playerName);
      localPlayer = this;
    }
  }

  [Command]
  public void SetName(string name)
  {
    this.playerName = name;
  }

  private void OnDestroy() {
    if (GameGlobals.globals.GetPlayers().Length == 1) {
      MatchManager.manager.EndGame();
    }
  }

  IEnumerator WaitForGlobals() {
    yield return new WaitUntil(() => GameGlobals.globals != null && this.playerName != null && this.playerName != "");
    print(this.playerName);
    GameGlobals.globals.GetPlayers();
  }

}
