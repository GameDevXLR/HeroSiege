using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playAudioSource : MonoBehaviour {

    public AudioClip source;

    public void play()
    {
        GetComponent<AudioSource>().PlayOneShot(source);
    }
}
