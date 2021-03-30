using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPoint : MonoBehaviour
{
    public Transform mouth;
    private float dist;

    void Start() {
      dist = transform.position.y - mouth.position.y;
    }
    void Update()
    {
        transform.position = mouth.position + new Vector3(0, dist, 0);
    }
}
