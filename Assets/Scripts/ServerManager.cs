using System.Collections;
using UnityEngine;
using Mirror;


public class ServerManager : NetworkManager
{
  public string connectIp = "";
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
    ServerManager.server.StartHost();
    lobbyUI = Instantiate(lobbyUIPrefab) as GameObject;
    NetworkServer.Spawn(lobbyUI);
  }

  public void Join() {
    ServerManager.server.StartClient(new System.Uri($"kcp://{connectIp}"));
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

  public override void OnClientConnect() {
    base.OnClientConnect();
    StartCoroutine(WaitForLobbySpawn());
  }


  IEnumerator WaitForLobbySpawn() {
    yield return new WaitUntil(() => GameObject.FindObjectOfType<GameGlobals>());
    lobbyUI = GameObject.FindObjectOfType<GameGlobals>().gameObject;
  }

}
