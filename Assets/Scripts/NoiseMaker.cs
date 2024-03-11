using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseMaker : MonoBehaviour
{
    public AudioSource source;
    //public AudioClip clip;

    public void MakeNoise(AudioClip clip)
    {
        source.clip = clip;
        source.Play();
    }
}
