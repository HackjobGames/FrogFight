using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
       Lobby.roomNumber = roomInput.text;
       playerName = nameEntry.text;
       SceneManager.LoadScene("Lobby");
    }
}
