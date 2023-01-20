using UnityEngine.UI;
using Mirror;

public class Lobby : NetworkBehaviour
{
  public Image menu;
  public Button menuButton;

  public static Lobby lobby;

  private void Start() {
    lobby = this;
    menu.gameObject.SetActive(false);
    if (this.isServer) {
      menuButton.interactable = true;
    }
  }

  public void toggleMenu(bool enable) {
    menu.gameObject.SetActive(enable);
  }

  public void Disconnect() {
    ServerManager.server.Disconnect();
  }
    
}
