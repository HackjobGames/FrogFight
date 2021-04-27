using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;

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
    public static string playerName;
    public static MainMenu menu;

    private void Start() {
      menu = this;
    }

    public void toggleHostDialog(bool status) {
      hostDialog.SetActive(status);
    }

    public void togglePasswordDialog(bool status) {
      passwordDialog.SetActive(status);
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

    public void Join() {
      playerName = nameEntry.text;
      ServerManager.server.matchID = roomInput.text;
      ServerManager.server.password = joinPassword.text;
      ServerManager.server.Join();
    }
}
