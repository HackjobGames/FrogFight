using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;
public class TestZoneScript : MonoBehaviour
{
  public Obi.ObiRope rope;

    void Start()
    {
     // rope.path.RemoveControlPoint(0);
      ServerManager.server.StartHost();
      StartCoroutine(WaitASec());
    }
    IEnumerator WaitASec(){
      yield return new WaitForSeconds(1.0f);
      GameObject player = GameObject.Find("Player [connId=0]");
      player.GetComponent<Character>().enabled = true;
    }
}
