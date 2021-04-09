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

    private void Start() {
      menu.gameObject.SetActive(false);
      if (this.isServer) {
        menuButton.interactable = true;
      }
      StartCoroutine(WaitForID());
    }

    public void toggleMenu(bool enable) {
      menu.gameObject.SetActive(enable);
    }
    IEnumerator WaitForID()
    {
      yield return new WaitUntil(() => ServerManager.matchID != null);
      codeDisplay.text = "Code: " + ServerManager.matchID;
    }
    
}
