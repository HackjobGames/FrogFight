using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAudio : MonoBehaviour
{
  public AudioSource click;
  public static UIAudio UIclick;

  private void Start() {
    click = GetComponent<AudioSource>();
    UIclick = this;
  }

  public void Click() {
    click.Play();
  }

}
