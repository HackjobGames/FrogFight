using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AddNetworkIdentity : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.AddComponent<NetworkIdentity>();
    }

}
