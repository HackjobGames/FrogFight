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
    public static string playerName;
    public void Host() {
      playerName = nameEntry.text;
      SceneManager.LoadScene("Lobby");
    }

    public void Join() {
      playerName = nameEntry.text;
      ServerManager.matchID = roomInput.text;
      SceneManager.LoadScene("Lobby");
    }
}
