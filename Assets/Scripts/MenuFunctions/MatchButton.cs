using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchButton : MonoBehaviour
{
    public Match match;

    public void JoinMatch() {
        MainMenu.menu.JoinFromMatch(match);
    }
}
