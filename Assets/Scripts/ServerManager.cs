using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class ServerManager : MonoBehaviour
{
    public static string roomNumber = null;
    public DarkReflectiveMirrorTransport transport;
    public NetworkManager network;

    private void Start() {
      if (roomNumber == null) {
        network.StartHost();
        roomNumber = "" + transport.serverID;
      } else {
        network.networkAddress = roomNumber;
        network.StartClient();
      }
    }
}
