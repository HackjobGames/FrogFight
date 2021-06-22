using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyBox : MonoBehaviour
{
    public bool enableSkybox;
    public bool enableFog;
    public Material skyMaterial;
    public Color fogColor;
    public float fogDensity;

    void Start()
    {
      if(enableSkybox){
        RenderSettings.skybox = skyMaterial; 
      } else {
        RenderSettings.skybox = null;
      }
      
      if(enableFog){
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.ExponentialSquared;
        RenderSettings.fogDensity = fogDensity;
        RenderSettings.fogColor = fogColor;
      } else {
        RenderSettings.fog = false;
      }
    }
}
