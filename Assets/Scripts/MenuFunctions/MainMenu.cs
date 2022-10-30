using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Text;
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
      print(roomInput.text.ToUpper());
      roomInput.text = roomInput.text.ToUpper();
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

    public void toggleIsPrivate() {
      hostPassword.interactable = isPrivate.isOn;
      hostPassword.text = isPrivate.isOn ? hostPassword.text : "";
    }

    public void Host() {
      ServerManager.server.isPrivate = isPrivate.isOn;
      ServerManager.server.password = hostPassword.text;
      ServerManager.server.maxPlayers = maxPlayers.value + 2;
      ServerManager.server.Host();
    }

    public void Quit() {
      Application.Quit();
    }

    public void Join() {
      if (roomInput.text.Length == 6) {
        ServerManager.server.matchID = roomInput.text;
        ServerManager.server.password = joinPassword.text;
        ServerManager.server.Join();
        joinError.text = "";
      } else {
        joinError.text = "Match ID must be 6 digits long.";
      }
    }
}
