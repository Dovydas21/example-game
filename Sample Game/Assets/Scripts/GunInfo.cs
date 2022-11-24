using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
[RequireComponent(typeof(AudioSource))]
public class GunInfo : MonoBehaviour
{
    // GunInfo.cs
    //  Purpose of this script is to serve as a modular way to store stats, anmations, particle effects, sound effects (etc)
    //  that relate to guns. This is so that Shoot.cs (and any future scripts that wish to read stats / amend stats of any
    //  given gun) can all point to this one script.

    [Header("Gun characteristics")]
    public bool fullAuto;                                       // True = Gun can be fired continuously while mouse is held down. False = semi-auto.
    public bool hasToBeCocked;                                  // True = Gun must be cocked (e.g. Shotgun / Revolver), False = Gun is semi-auto.

    [Header("Gun stats")]
    public float power;                                         // The force multiplier applied when the gun lands a hit on a RigidBody.
    public float range;                                         // The range of the raycast shot when the gun is fired.
    public float fireRate;                                      // The fire rate of the gun. .1 is fast 10 is slow.
    public int magCapacity;                                     // The gun's capacity of rounds that it can fire before reloading.
    public int damage;                                          // Damage the gun does when it hits enemies.
    public int ammoInGun;

    [Header("Gun positions")]
    public GameObject gunHolder;                                // Gameobject attached to the camera that holds the gun.
    public Vector3 defaultGunPosition, defaultGunAngles;        // The local position & rotation of the gun inside of gunHolder when NOT aimed in.
    bool playerAimedDownSights = false;                         // Keeps track of whether the player is currently aiming down their sights.

    [Header("Gun animations")]
    public Animator shootingAnimation;                          // The animaTOR that has the animation the gun should play when the gun is fired.
    public Animator cockingAnimation;                           // The animaTOR that has the animation the gun should play when the gun is being cocked.
    public ParticleSystem muzzleFlash;                          // The particle effect that should serve as the muzzle flash when the gun is fired.
    public TrailRenderer bulletTrail;                           // The TrailRenderer asset that immitates the travel of a tracer round, when the gun is fired.
    public float bulletTrailSpeed = 1000f;                      // The speed that the trail moves to the hitposition in Shoot.cs.

    [Header("Gun audio")]
    public AudioSource shotSoundEffect;                         // The audio source of the sound effect that should play when the gun is fired.
    public AudioSource cockingSoundEffect;                      // The audio source of the sound effect that should play when the gun is cocked.

    [Header("Misc components")]
    public BoxCollider pickupTrigger;                           // The box collider that triggers the player to pick up the gun if it is on the ground.
    public Shoot shootScript;
    public GunRecoil gunRecoilScript;
    public GameObject gunObj;
    GameObject playerObj;
    


    Rigidbody rb;
    float gunMass, gunDrag;
    bool gunGravity;

    private void Start()
    {
        ammoInGun = magCapacity; // Set the current ammo count to be the mag capacity (i.e. fully loaded).
        gunHolder = GameObject.FindGameObjectWithTag("GunHolder");
        playerObj = GameObject.FindGameObjectWithTag("Player");
        shootScript = playerObj.GetComponent<Shoot>();
        rb = gameObject.GetComponent<Rigidbody>();

        // Remember properties about the rigidbody of the object.
        gunMass = rb.mass;
        gunDrag = rb.drag;
        gunGravity = rb.useGravity;
    }

    private void OnTriggerEnter(Collider pickupTrigger)
    {
        if (pickupTrigger.tag == "Player" && gunHolder.transform.childCount == 0) // Trigger a pick-up if you are the player and you are not already holding something.
        {
            gunObj = gameObject;
            gunObj.transform.parent = gunHolder.transform;
            //gunObj.transform.position = gunHolder.transform.position;
            // gunObj.transform.rotation = gunHolder.transform.rotation;
            gunObj.transform.localEulerAngles = defaultGunAngles;
            gunObj.transform.localPosition = defaultGunPosition;

            playerObj.GetComponent<InputManager>().gunInfo = this;

            Destroy(gameObject.GetComponent<Rigidbody>());              // Disable the rigidbody on the object.
            gameObject.GetComponent<BoxCollider>().enabled = false;     // Disable the box collider that was used to trigger the pickup.

            shootScript.Refresh(); // Refresh the shoot script to give it information about the gun just picked up.
            UpdateAmmoInGun(ammoInGun);
        }
    }

    public IEnumerator Drop()
    {
        if (!playerAimedDownSights)
        {
            gameObject.transform.parent = null;

            // Set the Rigidbody values back to what they were before we destroyed it.
            rb = gameObject.AddComponent<Rigidbody>();
            rb.mass = gunMass;
            rb.drag = gunDrag;
            rb.useGravity = gunGravity;

            rb.AddForce(Vector3.forward * 10f, ForceMode.Force);
            ResetAmmoCounter(); // Hide the ammo counter.
            shootScript.Refresh(); // Refresh the shoot script to give it information about the gun just picked up / dropped.

            yield return new WaitForSeconds(1f); // Wait for 1 second before re-enabling the pickup-trigger.
            gameObject.GetComponent<BoxCollider>().enabled = true; // Re-enable the pickup trigger.
        }
    }

    public void ResetAmmoCounter()
    {
        GameObject.Find("AmmoCounter").GetComponent<Text>().text = "";
    }

    public void UpdateAmmoInGun(int value)
    {
        ammoInGun = value;
        GameObject.Find("AmmoCounter").GetComponent<Text>().text = ammoInGun.ToString();
        // if(value == ammoInGun) 
    }

    public void PlayCockingAnimation()
    {
        if (cockingAnimation != null)
            cockingAnimation.SetTrigger("Fire");
    }

    IEnumerator PlayAndWaitForSound(AudioSource soundSource)
    {
        soundSource.Play();
        while (soundSource.isPlaying) //Wait Until Sound has finished playing
            yield return new WaitForSeconds(.01f);

        yield return true;
    }

    public void PlayShootAnimation()
    {
        if (shootingAnimation != null)
            shootingAnimation.SetTrigger("Fire");
    }

    public void PlayCockingSound()
    {
        if (cockingSoundEffect != null)
            cockingSoundEffect.Play();
    }

    public void PlayShootSound() // Plays the sound effect that is set up on the 
    {
        if (shotSoundEffect != null)
        {
            StartCoroutine(PlayAndWaitForSound(shotSoundEffect));
            if (hasToBeCocked) PlayCockingSound();
        }
    }

    public void PlayMuzzleFlash()
    {
        if (muzzleFlash != null)
            muzzleFlash.Play();
    }
}
