using UnityEngine;
using UnityEngine.UI;
using HSVPicker;

public class MainMenu : MonoBehaviour
{
    public Text roomInput;
    public InputField nameEntry;
    public GameObject hostDialog;
    public InputField hostPassword;
    public InputField joinPassword;
    public Toggle isPrivate;
    public Dropdown maxPlayers;
    public GameObject passwordDialog;
    public GameObject menuCamera;
    public GameObject mainMenuUi;
    public Text apiError;
    public Text joinError;
    public GameObject errorDialog;
    public GameObject joinDialog;
    public GameObject buttonContainer;
    public GameObject inGameMenu;
    public GameObject settingsMenu;
    public ColorPicker picker;
    public static MainMenu menu;
    

    private void Start() {
      Save.Load();
      nameEntry.text = Save.save.name;
      picker.AssignColor(new Color(Save.save.color[0], Save.save.color[1], Save.save.color[2]));
      menu = this;
    }

    void Update() {
      if(MatchManager.manager!= null){
        if (Input.GetButtonDown("Cancel") && MatchManager.manager.inGame) {
          Cursor.lockState = inGameMenu.activeSelf && MatchManager.manager.inGame ? CursorLockMode.Locked : CursorLockMode.None;
          inGameMenu.SetActive(!inGameMenu.activeSelf);
        } 
      }
    }

    public void OnColorChange() {
      if (Save.save != null) {
        Save.save.color = new float[] {
          picker.R,
          picker.G,
          picker.B,
        };
        Save.SaveGame();
      }
    }

    public void ToggleSettings(){
      if(settingsMenu.activeSelf){
        settingsMenu.SetActive(!settingsMenu.activeSelf);
        settingsMenu.GetComponent<Settings>().SaveSettings();
      } else {
        settingsMenu.SetActive(!settingsMenu.activeSelf);
        settingsMenu.GetComponent<Settings>().LoadSettings();
      }
    }
    
    public void UpdatePlayerName() {
      Save.save.name = nameEntry.text;
      Save.SaveGame();
    }

    public void Disconnect() {
      ServerManager.server.Disconnect();
      inGameMenu.SetActive(false);
    }

    public void closeInGameMenu() {
      inGameMenu.SetActive(false);
      Cursor.lockState = CursorLockMode.Locked;
    }

    public void UpdateRoomCode() {
      roomInput.text = roomInput.text;
    }

    public void toggleErrorDialog(bool status) {
      errorDialog.SetActive(status);
    }

    public void toggleHostDialog(bool status) {
      hostDialog.SetActive(status);
    }

    public void togglePasswordDialog(bool status) {
      passwordDialog.SetActive(status);
    }

    public void toggleJoinDialog(bool status) {
      joinDialog.SetActive(status);
      joinError.text = "";
    }

    public void Host() {
      ServerManager.server.maxPlayers = maxPlayers.value + 2;
      ServerManager.server.Host();
    }

    public void Quit() {
      Application.Quit();
    }

    public void Join() {
      ServerManager.server.connectIp = roomInput.text;
      ServerManager.server.Join();
    }
}
