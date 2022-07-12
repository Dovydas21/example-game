using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SFXScript : MonoBehaviour
{
    public AudioSource jumpSoundEffect;

    public void PlayGrunt()
    {
        Debug.Log(jumpSoundEffect);
        jumpSoundEffect.Play();
    }
}
