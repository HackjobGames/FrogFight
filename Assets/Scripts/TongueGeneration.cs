using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TongueGeneration : MonoBehaviour
{
    public int length = 40;
    public GameObject segment;
    GameObject current;
    void Start()
    {
      current = Instantiate(segment);
      for(int i = 1; i < length; i++) {
        HingeJoint joint = current.GetComponent<HingeJoint>();
        GameObject next = Instantiate(segment);
        next.transform.position = new Vector3(next.transform.position.x + i * .30f, next.transform.position.y, next.transform.position.z);
        joint.connectedArticulationBody = next.GetComponent<ArticulationBody>();
        joint.connectedBody = next.GetComponent<Rigidbody>();
        current = next;
      }
      Destroy(current.GetComponent<HingeJoint>());
    }
    void Update()
    {
    }
}
