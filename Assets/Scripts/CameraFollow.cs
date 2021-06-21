using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform lookAt;
    private Camera cam;

    private float distance = 10.0f;
    private float currentX = 0.0f;
    private float currentY = 0.0f;
    private float sensitivityX = 4.0f;
    private float sensitivityY = 4.0f;

    public bool cameraWall = false;

    void Start() {
      cam = GetComponent<Camera>();
    }

    void Update() {
      currentX += Input.GetAxis("Mouse X");
      currentY -= Input.GetAxis("Mouse Y");
      distance -= Input.GetAxis("Mouse ScrollWheel");
      currentY = Mathf.Clamp(currentY, -60.0f, 90.0f);
    }

    void LateUpdate() {
      if(!cameraWall){
        Vector3 dir = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        transform.position = lookAt.position + rotation * dir;
      }
        transform.LookAt(lookAt.position);
    }

    private void OnTriggerEnter(Collider other) {
      print(other.tag);
      if(other.tag == "CameraWall"){
        cameraWall = true;
      }
    }
}
