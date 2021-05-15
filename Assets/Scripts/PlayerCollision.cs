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
    public SphereCollider player_head;

    void Start()
    {
        distance_to_ground = player_head.radius * 12;
    }

    void Update()
    {
      if (this.isLocalPlayer) {
        if(Physics.Raycast(player_head.gameObject.transform.position,Vector3.down,distance_to_ground + .1f, ground_layer)){
            on_ground = true;
        } else {
            on_ground = false;
        }

        if(Physics.Raycast(player_head.gameObject.transform.position,Vector3.up,distance_to_ground + .1f)){
            on_ceiling = true;
        } else {
            on_ceiling = false;
        }
      }
    }
    void OnDrawGizmos()
    {
        Debug.DrawRay((Vector3)transform.position, Vector3.down * distance_to_ground);
        Debug.DrawRay((Vector3)transform.position, Vector3.up * distance_to_ground);
    }
}