using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[RequireComponent(typeof(AudioSource))]
public class GunInfo : MonoBehaviour
{
    // GunInfo.cs
    //  Purpose of this script is to serve as a modular way to store stats, anmations, particle effects, sound effects (etc)
    //  that relate to guns. This is so that Shoot.cs (and any future scripts that wish to read stats / amend stats of any
    //  given gun) can all point to this one script.
    
    public bool fullAuto;                                       // True = Gun can be fired continuously while mouse is held down. False = semi-auto.
    public bool hasToBeCocked;                                  // True = Gun must be cocked (e.g. Shotgun / Revolver), False = Gun is semi-auto.
    public float power;                                         // The force multiplier applied when the gun lands a hit on a RigidBody.
    public float range;                                         // The range of the raycast shot when the gun is fired.
    public float fireRate;                                      // The fire rate of the gun. .1 is fast 10 is slow.
    public float aimedInFOV;                                    // The FOV of the camera when aiming in with this gun.
    public GameObject gunHolder;                                       // Gameobject attached to the camera that holds the gun.
    public Vector3 aimedInPosition, aimedInAngle;               // The local position & rotation of the gun inside of gunHolder when aimed in.
    public Vector3 defaultGunPosition, defaultGunAngles;        // The local position & rotation of the gun inside of gunHolder when NOT aimed in.
    public Animator shootingAnimation;                          // The animaTOR that has the animation the gun should play when the gun is fired.
    public Animator cockingAnimation;                           // The animaTOR that has the animation the gun should play when the gun is being cocked.
    public AudioSource shotSoundEffect;                         // The audio source of the sound effect that should play when the gun is fired.
    public ParticleSystem muzzleFlash;                          // The particle effect that should serve as the muzzle flash when the gun is fired.
    public BoxCollider pickupTrigger;                           // The box collider that triggers the player to pick up the gun if it is on the ground.
    public Shoot shootScript;

    private void Start()
    {
        gunHolder = GameObject.FindGameObjectWithTag("GunHolder");
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        shootScript = playerObj.GetComponent<Shoot>();
    }

    private void OnTriggerEnter(Collider pickupTrigger)
    {
        if (pickupTrigger.tag == "Player") // Trigger a pick-up if you are the player.
        {
            gameObject.transform.SetParent(gunHolder.transform);
            gameObject.transform.position = gunHolder.transform.position;
            gameObject.transform.rotation = gunHolder.transform.rotation;
            Destroy(gameObject.GetComponent<Rigidbody>());
            shootScript.Refresh(); // Refresh the shoot script to give it information about the gun just picked up.
        }
    }

    public void PlayCockingAnimation()
    {
        if(cockingAnimation != null)
            cockingAnimation.SetTrigger("Fire");
    }

    public void PlayShootAnimation()
    {
        if (shootingAnimation != null)
            shootingAnimation.SetTrigger("Fire");
    }

    public bool PlayShootSound() // Plays the sound effect that is set up on the 
    {
        if (shotSoundEffect != null)
        {
            bool shotSound = shotSoundEffect.isPlaying;
            if (shotSound != true)
                shotSoundEffect.Play();
            return shotSound != true;
        }
        else return false;
    }

    public void PlayMuzzleFlash()
    {
        if(muzzleFlash != null)
            muzzleFlash.Play();
    }
}
