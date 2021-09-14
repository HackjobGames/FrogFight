using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Text;
using Newtonsoft.Json;

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
    public GameObject matchButtonPrefab;
    public GameObject buttonContainer;
    public GameObject matchButtonDialog;
    public GameObject inGameMenu;
    public GameObject settingsMenu;
    public static MainMenu menu;
    

    private void Start() {
      Save.Load();
      menu = this;
      nameEntry.text = Save.name;
    }

    void Update() {
      if(MatchManager.manager!= null){
        if (Input.GetButtonDown("Cancel") && MatchManager.manager.inGame) {
          Cursor.lockState = inGameMenu.activeSelf && MatchManager.manager.inGame ? CursorLockMode.Locked : CursorLockMode.None;
          inGameMenu.SetActive(!inGameMenu.activeSelf);
        } 
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
      Save.name = nameEntry.text;
      Save.SaveName();
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

    public void toggleMatchDialog(bool status) {
      matchButtonDialog.SetActive(status);
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

    public void Play() {
      matchButtonDialog.SetActive(true);
      StartCoroutine(GetMatches());
    }

    public void Refresh() {
      StartCoroutine(GetMatches());
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

    
    public void JoinFromMatch(Match match) {
      ServerManager.server.matchID = match.MatchID;
      ServerManager.server.password = joinPassword.text;
      ServerManager.server.Join();
    }

    IEnumerator GetMatches() {
      foreach (Transform obj in buttonContainer.transform) {
        Destroy(obj.gameObject);
      }
      UnityWebRequest req = UnityWebRequest.Get($"http://66.41.159.125:8090/getMatches");
      yield return req.SendWebRequest();
      int ix = 0;
      Match[] matches = JsonConvert.DeserializeObject<Match[]>(Encoding.UTF8.GetString(req.downloadHandler.data));
      RectTransform containerRect = buttonContainer.GetComponent<RectTransform>();
      containerRect.sizeDelta = new Vector2(containerRect.sizeDelta.x, matchButtonPrefab.GetComponent<RectTransform>().sizeDelta.y * matches.Length + 10);
      foreach(Match match in matches) {
        GameObject button = Instantiate(matchButtonPrefab) as GameObject;
        button.transform.SetParent(buttonContainer.transform);
        button.GetComponent<MatchButton>().SetMatch(match);
        RectTransform rect = button.GetComponent<RectTransform>();
        rect.localScale = new Vector3(1,1,1);
        rect.anchoredPosition3D = new Vector3(-10, -matchButtonPrefab.GetComponent<RectTransform>().rect.height * ((float)ix - (.5f * (matches.Length - 1))), 0);
        ix++;
      }
    }
}
