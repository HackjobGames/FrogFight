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
    public Text nameEntry;
    public GameObject hostDialog;
    public InputField hostPassword;
    public InputField joinPassword;
    public Toggle isPrivate;
    public Dropdown maxPlayers;
    public GameObject passwordDialog;
    public GameObject menuCamera;
    public GameObject mainMenuUi;
    public Text apiError;
    public GameObject errorDialog;
    public GameObject matchButtonPrefab;
    public GameObject buttonContainer;
    public GameObject matchButtonDialog;
    public GameObject inGameMenu;
    public static string playerName;
    public static MainMenu menu;
    

    private void Start() {
      menu = this;
    }

    void Update() {
      if (Input.GetButtonDown("Cancel") && MatchManager.manager.inGame) {
        Cursor.lockState = inGameMenu.active && MatchManager.manager.inGame ? CursorLockMode.Locked : CursorLockMode.None;
        inGameMenu.SetActive(!inGameMenu.active);
      }
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

    public void toggleIsPrivate() {
      hostPassword.interactable = isPrivate.isOn;
      hostPassword.text = isPrivate.isOn ? hostPassword.text : "";
    }

    public void Host() {
      playerName = nameEntry.text;
      ServerManager.server.isPrivate = isPrivate.isOn;
      ServerManager.server.password = hostPassword.text;
      ServerManager.server.maxPlayers = maxPlayers.value + 2;
      ServerManager.server.Host();
    }

    public void Play() {
      StartCoroutine(GetMatches());
    }

    public void Join() {
      playerName = nameEntry.text;
      ServerManager.server.matchID = roomInput.text;
      ServerManager.server.password = joinPassword.text;
      ServerManager.server.Join();
    }

    
    public void JoinFromMatch(Match match) {
      playerName = nameEntry.text;
      ServerManager.server.matchID = match.MatchID;
      ServerManager.server.password = joinPassword.text;
      ServerManager.server.Join();
    }

    IEnumerator GetMatches() {
      UnityWebRequest req = UnityWebRequest.Get($"http://localhost:8090/getMatches");
      yield return req.SendWebRequest();
      matchButtonDialog.SetActive(true);
      
      int ix = 0;
      IDictionary<string, Match> matches = JsonConvert.DeserializeObject<IDictionary<string, Match>>(Encoding.UTF8.GetString(req.downloadHandler.data));
      RectTransform containerRect = buttonContainer.GetComponent<RectTransform>();
      containerRect.sizeDelta = new Vector2(containerRect.sizeDelta.x, matchButtonPrefab.GetComponent<RectTransform>().sizeDelta.y * matches.Count + 10);
      foreach(KeyValuePair<string, Match> match in matches) {
        GameObject button = Instantiate(matchButtonPrefab) as GameObject;
        button.transform.SetParent(buttonContainer.transform);
        button.GetComponent<MatchButton>().SetMatch(match.Value);
        RectTransform rect = button.GetComponent<RectTransform>();
        rect.localScale = new Vector3(1,1,1);
        rect.anchoredPosition3D = new Vector3(0, -matchButtonPrefab.GetComponent<RectTransform>().rect.height * ((float)ix - (.5f * (matches.Count - 1))), 0);
        ix++;
      }
    }
}
