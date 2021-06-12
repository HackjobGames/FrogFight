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
  DarkReflectiveMirrorTransport darkTransport;

  public override void Start() {
    base.Start();
    darkTransport = GetComponent<DarkReflectiveMirrorTransport>();
    server = this;
  }

  public void Host() {
    ServerManager.server.StartHost();
    StartCoroutine(GetHostID(darkTransport.serverID));
  }

  public void Join() {
    StartCoroutine(TryConnect());
  }
  public void Disconnect() {
    if (MatchManager.manager.isServer) {
      ServerManager.server.StopHost();
    } else {
      ServerManager.server.StopClient();
    }
  }

  IEnumerator GetHostID(int serverID) {
    string privateString = isPrivate ? "true" : "false";
    UnityWebRequest req = UnityWebRequest.Get($"http://localhost:8090/host?relayID={serverID}&hostName={MainMenu.playerName}&isPrivate={privateString}&password={password}&maxPlayers={maxPlayers}");
    yield return req.SendWebRequest();

    if(req.result != UnityWebRequest.Result.Success){
      print(req.error);
    } else {
      print(Encoding.UTF8.GetString(req.downloadHandler.data));
      matchID = Encoding.UTF8.GetString(req.downloadHandler.data);
      lobbyUI = Instantiate(lobbyUIPrefab) as GameObject;
      NetworkServer.Spawn(lobbyUI);
    }
  }

  IEnumerator TryConnect() {
    UnityWebRequest req = UnityWebRequest.Get($"http://localhost:8090/join?matchID={matchID}&password={password}");
    yield return req.SendWebRequest();
    string responseMessage = Encoding.UTF8.GetString(req.downloadHandler.data);
    if(req.result != UnityWebRequest.Result.Success) {
      MainMenu.menu.apiError.text = responseMessage;
      MainMenu.menu.errorDialog.SetActive(true);
    } else {
      if (responseMessage != "private") {
        networkAddress = responseMessage;
        StartClient();
        StartCoroutine(WaitForLobbySpawn());
        MainMenu.menu.passwordDialog.SetActive(false);
        MainMenu.menu.mainMenuUi.SetActive(false);
      } else {
        MainMenu.menu.passwordDialog.SetActive(true);
      }
    }
  }
  IEnumerator WaitForLobbySpawn() {
    yield return new WaitUntil(() => GameObject.FindObjectOfType<GameGlobals>());
    lobbyUI = GameObject.FindObjectOfType<GameGlobals>().gameObject;
  }

}
