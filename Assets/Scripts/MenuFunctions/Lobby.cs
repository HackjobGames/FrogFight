using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Lobby : MonoBehaviour
{
    public static string roomNumber = null;
    public Text codeDisplay;
    public DarkReflectiveMirrorTransport transport;
    public NetworkManager network;

    private void Start() {
      if (roomNumber == null) {
        network.StartHost();
        roomNumber = "" + transport.serverID;
        codeDisplay.text = "Code: " + roomNumber;
      } else {
        network.networkAddress = roomNumber;
        codeDisplay.text = "Code: " + roomNumber;
        network.StartClient();
      }
    }
}
