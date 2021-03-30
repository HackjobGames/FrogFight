using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Impact : MonoBehaviour
{
  private float force;

  void Start() {
    force = GameObject.Find("Globals").GetComponent<GameGlobals>().slam_power;
  }

  void OnCollisionEnter(Collision other) {
    if (other.gameObject.tag == "Terrain" && other.relativeVelocity.magnitude > 15) {
      Collider[] colliders = Physics.OverlapSphere(transform.position, other.relativeVelocity.magnitude);
      foreach (Collider nearbyObject in colliders) {
        Character player = nearbyObject.GetComponentInParent<Character>();
        if (player != null) {
          player.AddShockWave(force, transform.position, other.relativeVelocity.magnitude);
        }
      }
    }
  }
}
