using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour
{
  [SyncVar]
  public string playerName;

  [SyncVar]
  public bool loaded;

  public static Player localPlayer;
  
  private void Start() {
    if (this.isLocalPlayer) {
      this.playerName = MainMenu.playerName;
      localPlayer = this;
    }
  }
  public override void OnStartLocalPlayer() {
    SetName(MainMenu.playerName);
  }

  [Command]
  public void SetName(string name)
  {
      this.playerName = name;
  }

  private void OnDestroy() {
    if (GameGlobals.GetPlayers().Length == 1) {
      MatchManager.manager.EndGame();
    }
  }

  
}
