using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class DestructableObject : NetworkBehaviour
{
  public GameObject effect;
  public Vector3 center;

  public void Destroy() {
    Instantiate(effect, center, effect.transform.rotation);
    Destroy(this.gameObject);
  }
}
