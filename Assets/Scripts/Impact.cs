using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Impact : MonoBehaviour
{
  private float delay = 1.0f;
  private bool canSlam = true;
  MatchManager manager;

  void Start() {
    manager = MatchManager.manager;
  }

  void OnCollisionEnter(Collision other) {
    if ((other.gameObject.tag == "Terrain" || other.gameObject.tag == "Player" && MatchManager.manager.inGame) && other.gameObject.GetComponentInParent<Impact>() != this && other.relativeVelocity.magnitude > 15 && canSlam) {
      Player.localPlayer.Impact(transform.position, other.relativeVelocity.magnitude, other.GetContact(0).normal);
      StartCoroutine(WaitForNextSlam());
    }
  }

  IEnumerator WaitForNextSlam() {
    canSlam = false;
    yield return new WaitForSeconds(delay);
    canSlam = true;
  }
}
