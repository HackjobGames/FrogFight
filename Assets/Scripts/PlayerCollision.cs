using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class PlayerCollision : NetworkBehaviour
{
    public bool on_ground;
    public bool on_ceiling;
    public LayerMask ground_layer;
    private float distance_to_ground;
    void Start()
    {
        distance_to_ground = GetComponent<CapsuleCollider>().height / 2;
    }

    void Update()
    {
      if (this.isLocalPlayer) {
        if(Physics.Raycast(transform.position,Vector3.down,distance_to_ground + .1f)){
            on_ground = true;
        } else {
            on_ground = false;
        }

        if(Physics.Raycast(transform.position,Vector3.up,distance_to_ground + .1f)){
            on_ceiling = true;
        } else {
            on_ceiling = false;
        }
      }
    }
    void OnDrawGizmos() {
        Debug.DrawRay((Vector3)transform.position, Vector3.down * distance_to_ground);
        Debug.DrawRay((Vector3)transform.position, Vector3.up * distance_to_ground);
    }
    
    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "Player") {
          Character character = other.gameObject.GetComponent<Character>();
          character.movement_machine.ChangeState(character.ragdoll_state);
        }
    }
}