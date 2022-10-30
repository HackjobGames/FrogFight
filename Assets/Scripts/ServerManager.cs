using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mirror;
using UnityEngine.Networking;
using System.Text;


public class ServerManager : NetworkManager
{
  public string matchID = null;
  public bool isPrivate = false;
  public string password = null;
  public int maxPlayers = 0;
  public NetworkManager network;
  public GameObject lobbyUIPrefab;
  public GameObject lobbyUI;
  public static ServerManager server;
  public Transport networkTransport;

  public override void Start() {
    base.Start();
    transport = gameObject.GetComponent<Transport>();
    server = this;
  }

  public void Host() {
    // ServerManager.server.StartHost();
    // StartCoroutine(GetHostID());
  }

  public void Join() {
    // StartCoroutine(TryConnect());
  }
  public void Disconnect() {
    if (MatchManager.manager.isServer) {
      ServerManager.server.StopHost();
    } else {
      ServerManager.server.StopClient();
    }
  }

  public void DestroyNetworkObject(GameObject obj) {
    NetworkServer.Destroy(obj);
  }

  public GameObject SpawnNetworkObject(GameObject obj) {
    obj = Instantiate(obj) as GameObject;
    NetworkServer.Spawn(obj);
    return obj;
  }

  IEnumerator WaitForLobbySpawn() {
    yield return new WaitUntil(() => GameObject.FindObjectOfType<GameGlobals>());
    lobbyUI = GameObject.FindObjectOfType<GameGlobals>().gameObject;
  }

}
