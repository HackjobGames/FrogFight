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
  public GameObject inGameMenu;

  public static Lobby lobby;

  public InputField tongueLengthInput;
  public InputField slamPowerInput;

  private void Start() {
    lobby = this;
    menu.gameObject.SetActive(false);
    if (this.isServer) {
      menuButton.interactable = true;
    }
    codeDisplay.text = "Code: " + ServerManager.server.matchID;
    SetDefaultSettings();
  }

  public void toggleMenu(bool enable) {
    menu.gameObject.SetActive(enable);
  }

  public void Disconnect() {
    ServerManager.server.Disconnect();
  }

  public void SetDefaultSettings() {
    print(GameGlobals.globals.slam_power);
    slamPowerInput.text = GameGlobals.globals.slam_power.ToString();
    tongueLengthInput.text = GameGlobals.globals.max_tongue_distance.ToString();
  }

  public void CopyCode() {
    GUIUtility.systemCopyBuffer = ServerManager.server.matchID;
  }
    
}
