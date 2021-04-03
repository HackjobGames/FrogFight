using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Impact : MonoBehaviour
{
  private float force;
  private float delay = 1.0f;
  private bool canSlam = true;
  MatchManager manager;

  void Start() {
    force = GameObject.Find("Globals").GetComponent<GameGlobals>().slam_power;
    manager = GameObject.Find("Globals").GetComponent<MatchManager>();
  }

  void OnCollisionEnter(Collision other) {
    
    if ((other.gameObject.tag == "Terrain" || other.gameObject.tag == "Player") && other.gameObject.GetComponentInParent<Impact>() != this && other.relativeVelocity.magnitude > 15 && canSlam) {
      StartCoroutine(WaitForNextSlam());
      Collider[] colliders = Physics.OverlapSphere(transform.position, other.relativeVelocity.magnitude);
      foreach (Collider nearbyObject in colliders) {
        Character player = nearbyObject.GetComponentInParent<Character>();
        Rigidbody body = nearbyObject.GetComponent<Rigidbody>();
        if (player) {
          player.AddShockWave(force, transform.position, other.relativeVelocity.magnitude, Quaternion.FromToRotation(Vector3.forward, other.GetContact(0).normal));
        } else if (body) {
          body.AddExplosionForce(force, transform.position, other.relativeVelocity.magnitude);
        } else if (nearbyObject.gameObject.tag == "Terrain") {
          manager.DestroyObject(nearbyObject.gameObject);
        }
      }
    }
  }

  IEnumerator WaitForNextSlam() {
    canSlam = false;
    yield return new WaitForSeconds(delay);
    canSlam = true;
  }
}
