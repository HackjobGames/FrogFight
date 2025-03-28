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
  public bool dead = true;

  [SyncVar]
  public bool loaded;

  [SyncVar]
  public int score;

  [SyncVar]
  public Color color;

  public static Player localPlayer;
  
  public void Start() {
    StartCoroutine(WaitForGlobals());
    if (this.isLocalPlayer) {
      SetName(Save.save.name);
      SetColor(new Color(Save.save.color[0], Save.save.color[1], Save.save.color[2]));
      localPlayer = this;
    }
  }

  [Command]
  public void SetName(string name)
  {
    this.playerName = name;
  }

  [Command]
  public void SetColor(Color color)
  {
    this.color = color;
  }

  private void OnDestroy() {
    if (GameGlobals.globals.GetPlayers().Length == 1) {
      MatchManager.manager.EndGame();
    }
  }

  [Command (requiresAuthority=false)]
  public void Impact(Vector3 origin, float magnitude, Vector3 normal) {
    ImpactClient(origin, magnitude, normal);
  }

  [ClientRpc]
  public void ImpactClient(Vector3 origin, float magnitude, Vector3 normal) {
    Collider[] players = Physics.OverlapSphere(origin, magnitude * 3);
    Collider[] terrain = Physics.OverlapSphere(origin, magnitude / 2);
    foreach (Collider nearbyObject in players) {
      Character player = nearbyObject.GetComponentInParent<Character>();
      if (player) {
        player.AddShockWave(GameGlobals.globals.slam_power, origin, magnitude, Quaternion.FromToRotation(Vector3.forward, normal));
      }
    }
    foreach(Collider nearbyObject in terrain) {
       if (nearbyObject.gameObject.tag == "Terrain") {
        nearbyObject.GetComponent<DestructableObject>().Destroy();
        if (GameGlobals.globals.game_mode == "Demolition") {
          score += 1;
          PlayerStatus.status.UpdateGame();
        }
      }
    }
  }

  IEnumerator WaitForGlobals() {
    yield return new WaitUntil(() => GameGlobals.globals != null && this.playerName != null && this.playerName != "");
    GameGlobals.globals.GetPlayers();
  }

}
