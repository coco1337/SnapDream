using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSoundController : MonoBehaviour
{
    private AudioSource sfx;

    public void TurnOnSound() => this.sfx.enabled = true;
    public void TurnOffSound() => this.sfx.enabled = false;
    // Start is called before the first frame update
    void Awake()
    {
        this.sfx = this.GetComponent<AudioSource>();
        this.TurnOffSound();
    }
}
