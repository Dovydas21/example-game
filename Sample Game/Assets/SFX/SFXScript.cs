using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SFXScript : MonoBehaviour
{
    public AudioSource jumpSoundEffect;
    public AudioSource shotSoundEffect;
    private bool shotShound;

    public void PlayGrunt()
    {
        jumpSoundEffect.Play();
    }

    public bool PlayShot()
    {
        bool shotSound = shotSoundEffect.isPlaying;
        if (shotSound != true) shotSoundEffect.Play();
        return shotSound != true;
    }
}
