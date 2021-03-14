using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour
{
  [SyncVar]
  public string playerName;
  
  private void Start() {
    if (this.isLocalPlayer) {
      this.playerName = MainMenu.playerName;
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

  
}
