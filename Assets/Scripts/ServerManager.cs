using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using UnityEngine.Networking;
using System.Text;


public class ServerManager : MonoBehaviour
{
    public static string matchID = null;
    public DarkReflectiveMirrorTransport transport;
    public static NetworkManager network;

    private void Start() {
      network = GetComponent<NetworkManager>();
      if (matchID == null) {
        network.StartHost();
        StartCoroutine(GetHostID(transport.serverID));
      } else {
        StartCoroutine(JoinOnMatchID());
      }
    }
    IEnumerator GetHostID(int serverID) {
      UnityWebRequest req = UnityWebRequest.Get($"http://localhost:8090/host?relayID={serverID}");
      yield return req.SendWebRequest();

      if(req.result != UnityWebRequest.Result.Success){
        print(req.error);
      } else {
        matchID = Encoding.UTF8.GetString(req.downloadHandler.data);
      }
    }

    IEnumerator JoinOnMatchID() {
      UnityWebRequest req = UnityWebRequest.Get($"http://localhost:8090/join?matchID={matchID}");
      yield return req.SendWebRequest();
      
      if(req.result != UnityWebRequest.Result.Success) {
        print(req.error);
      } else {
        network.networkAddress = Encoding.UTF8.GetString(req.downloadHandler.data);
        network.StartClient();
      }
    }
}
