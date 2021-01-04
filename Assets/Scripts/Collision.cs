using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour
{
    public bool on_ground;
    public LayerMask ground_layer;
    private float distance_to_ground;

    void Start()
    {
        distance_to_ground = GetComponent<CapsuleCollider>().height / 2;
    }

    void Update()
    {
        if(Physics.Raycast(transform.position,Vector3.down,distance_to_ground + .1f)){
            on_ground = true;
        } else {
            on_ground = false;
        }
    }
    void OnDrawGizmos()
    {
        Debug.DrawRay((Vector3)transform.position, Vector3.down * distance_to_ground);
    }
}