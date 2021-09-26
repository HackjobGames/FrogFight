using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestZoneScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
      ServerManager.server.StartHost();
      StartCoroutine(WaitASec());
    }
    IEnumerator WaitASec(){
      yield return new WaitForSeconds(1.0f);
      GameObject player = GameObject.Find("Player [connId=0]");
      player.GetComponent<Character>().enabled = true;
    }
}
