using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Lobby : NetworkBehaviour
{
    public Image menu;
    public Button menuButton;
    public Text codeDisplay;

    public static Lobby lobby;

    private void Start() {
      lobby = this;
      menu.gameObject.SetActive(false);
      if (this.isServer) {
        menuButton.interactable = true;
      }
      codeDisplay.text = "Code: " + ServerManager.server.matchID;
    }

    public void toggleMenu(bool enable) {
      menu.gameObject.SetActive(enable);
    }
    
}
