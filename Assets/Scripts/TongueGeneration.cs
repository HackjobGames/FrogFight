using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TongueGeneration : MonoBehaviour
{
    public int length = 10;
    public GameObject segment;
    GameObject current;
    void Start()
    {
      current = Instantiate(segment);
      for(int i = 1; i < length; i++) {
        SpringJoint joint = current.GetComponent<SpringJoint>();
        GameObject next = Instantiate(segment);
        next.transform.position = new Vector3(next.transform.position.x, next.transform.position.y + i, next.transform.position.z);
        joint.connectedBody = next.GetComponent<Rigidbody>();
        current = next;
      }
      Destroy(current.GetComponent<SpringJoint>());
    }
    void Update()
    {
      current.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}
