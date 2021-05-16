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
    public RectTransform containerMarker;
    public GameObject matchButtonDialog;
    public static string playerName;
    public static MainMenu menu;
    

    private void Start() {
      menu = this;
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
      foreach(KeyValuePair<string, Match> match in matches) {
        GameObject button = Instantiate(matchButtonPrefab) as GameObject;
        button.transform.parent = buttonContainer.transform;
        button.GetComponent<MatchButton>().match = match.Value;
        button.GetComponentInChildren<Text>().text = match.Value.HostName + "'s game.";
        RectTransform rect = button.GetComponent<RectTransform>();
        rect.localPosition = new Vector3(containerMarker.localPosition.x, containerMarker.localPosition.y - 70 - 127 * ix, containerMarker.localPosition.z);
        matchButtonPrefab.GetComponent<RectTransform>().localPosition += new Vector3(0, -127, 0);
        ix++;
      }
    }
}
