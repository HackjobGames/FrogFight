using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyBox : MonoBehaviour
{
    public Material sky_material;
    void Start()
    {
      RenderSettings.skybox = sky_material;   
    }
}
