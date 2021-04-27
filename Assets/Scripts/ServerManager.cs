using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mirror;
using UnityEngine.Networking;
using System.Text;


public class ServerManager : MonoBehaviour
{
    public string matchID = null;
    public bool isPrivate = false;
    public string password = null;
    public int maxPlayers = 0;
    public DarkReflectiveMirrorTransport transport;
    public NetworkManager network;
    public static ServerManager server;
    public GameObject lobbyUIPrefab;
    public static GameObject lobbyUI;
    public static GameObject lobbyCamera;

    private void Start() {
      server = this;
    }

    public void Host() {
      network.StartHost();
      StartCoroutine(GetHostID(transport.serverID));
    }

    public void Join() {
      StartCoroutine(TryConnect());
    }

    IEnumerator GetHostID(int serverID) {
      UnityWebRequest req = UnityWebRequest.Get($"http://localhost:8090/host?relayID={serverID}&isPrivate={isPrivate}&password={password}&maxPlayers={maxPlayers}");
      yield return req.SendWebRequest();

      if(req.result != UnityWebRequest.Result.Success){
        print(req.error);
      } else {
        print(Encoding.UTF8.GetString(req.downloadHandler.data));
        matchID = Encoding.UTF8.GetString(req.downloadHandler.data);
        lobbyUI = Instantiate(lobbyUIPrefab) as GameObject;
      }
    }

    IEnumerator TryConnect() {
      UnityWebRequest req = UnityWebRequest.Get($"http://localhost:8090/join?matchID={matchID}&password={password}");
      yield return req.SendWebRequest();
      
      if(req.result != UnityWebRequest.Result.Success) {
        print(req.error);
      } else {
        string responseMessage = Encoding.UTF8.GetString(req.downloadHandler.data);
        if (responseMessage != "private") {
          network.networkAddress = responseMessage;
          network.StartClient();
          lobbyUI = Instantiate(lobbyUIPrefab) as GameObject;
        } else {
          MainMenu.menu.passwordDialog.SetActive(true);
        }
      }
    }
}
