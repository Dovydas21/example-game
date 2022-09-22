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
    public Camera sightCamera;                                  // The camera that looks down the sight of the gun.
    bool playerAimedDownSights = false;                         // Keeps track of whether the player is currently aiming down their sights.

    [Header("Gun animations")]
    public Animator shootingAnimation;                          // The animaTOR that has the animation the gun should play when the gun is fired.
    public Animator cockingAnimation;                           // The animaTOR that has the animation the gun should play when the gun is being cocked.
    public ParticleSystem muzzleFlash;                          // The particle effect that should serve as the muzzle flash when the gun is fired.

    [Header("Gun audio")]
    public AudioSource shotSoundEffect;                         // The audio source of the sound effect that should play when the gun is fired.
    public AudioSource cockingSoundEffect;                      // The audio source of the sound effect that should play when the gun is cocked.

    [Header("Misc components")]
    public BoxCollider pickupTrigger;                           // The box collider that triggers the player to pick up the gun if it is on the ground.
    public Shoot shootScript;                                   // The shoot script that is attached to the player that controls when the gun is fired.
    GameObject playerObj;                                       // The GameObject that is the player.


    Rigidbody rb;
    float gunMass, gunDrag;
    bool gunGravity;
    bool allowPickup = true;

    private void OnValidate()
    {
        shootScript.Refresh();
    }

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

    private void OnTriggerEnter(Collider pickupTrigger) // Pickup weapon.
    {
        print("GunInfo.cs:OnTriggerEnter(Collider pickupTrigger):  Something has collided with " + gameObject.name);
        if (pickupTrigger.tag == "Player" && gunHolder.transform.childCount == 0 && allowPickup) // Trigger a pick-up if you are the player AND you are not already holding something AND you are allowed to pickup the weapon.
        {
            print("GunInfo.cs:OnTriggerEnter(Collider pickupTrigger):  Player has collided with " + gameObject.name + " starting pickup.");
            gameObject.transform.parent = gunHolder.transform;
            gameObject.transform.position = gunHolder.transform.position;
            gameObject.transform.rotation = gunHolder.transform.rotation;
            gameObject.transform.localEulerAngles = defaultGunAngles;
            gameObject.transform.localPosition = defaultGunPosition;
            print("GunInfo.cs:OnTriggerEnter(Collider pickupTrigger):  Set " + gameObject.name + "'s parent to GunHolder gameobject and adjusted the weapon pos accordingly.");


            Destroy(gameObject.GetComponent<Rigidbody>());              // Disable the rigidbody on the object.
            print("GunInfo.cs:OnTriggerEnter(Collider pickupTrigger):  Removed Rigidbody from " + gameObject.name);

            gameObject.GetComponent<BoxCollider>().enabled = false;     // Disable the box collider that was used to trigger the pickup.
            print("GunInfo.cs:OnTriggerEnter(Collider pickupTrigger):  Disabled the pickup-trigger on " + gameObject.name);



            shootScript.Refresh(); // Refresh the shoot script to give it information about the gun just picked up.
            print("GunInfo.cs:OnTriggerEnter(Collider pickupTrigger):  Refreshed 'Shoot.cs'");

            UpdateAmmoInGun(ammoInGun);
            print("GunInfo.cs:OnTriggerEnter(Collider pickupTrigger):  Calling 'UpdateAmmoInGun(ammoInGun)'");
        } else print("GunInfo.cs:OnTriggerEnter(Collider pickupTrigger):  Object that collided with " + gameObject.name + " is not a player.");
    }

    public IEnumerator Drop() // Drop weapon.
    {
        print("GunInfo.cs:Drop():  Dropping weapon...");

        if (!playerAimedDownSights)
        {
            print("GunInfo.cs:Drop():  Setting parent of " + gameObject.name + " to null.");
            gameObject.transform.parent = null;

            // Set the Rigidbody values back to what they were before we destroyed it.
            rb = gameObject.AddComponent<Rigidbody>();
            print("GunInfo.cs:Drop():  Added rigidbody to " + gameObject.name);
            rb.mass = gunMass;
            rb.drag = gunDrag;
            rb.useGravity = gunGravity;
            print("GunInfo.cs:Drop():  Set rigidbody components: rb.mass = " + rb.mass + ", rb.drag = " + rb.drag + ", rb.useGravity  = " + rb.useGravity);

            rb.AddForce(Vector3.forward * 10f, ForceMode.Force);
            print("GunInfo.cs:Drop():  Added force to " + gameObject.name);
            ResetAmmoCounter(); // Hide the ammo counter.
            print("GunInfo.cs:Drop():  Hidden ammo counter on UI.");
            shootScript.Refresh(); // Refresh the shoot script to give it information about the gun just picked up / dropped.
            print("GunInfo.cs:Drop():  Refreshed 'Shoot.cs' so it knows the player is no longer holding a weapon.");

            print("GunInfo.cs:Drop():  Waiting for 2 seconds before re-enabling the weapon's pickup collider.");
            allowPickup = false;
            yield return new WaitForSeconds(2f); // Wait for 1 second before re-enabling the pickup-trigger.

            print("GunInfo.cs:Drop():  re-enabling the weapon's pickup collider.");
            gameObject.GetComponent<BoxCollider>().enabled = true; // Re-enable the pickup trigger.
            allowPickup = true;

            print("GunInfo.cs:Drop():  Finished dropping weapon.");
        }
        else print("GunInfo.cs:Drop():  Cannot drop weapon because player is aiming down sights.");
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

    public void ToggleAim(bool aiming)
    {
        sightCamera.enabled = aiming;
        playerAimedDownSights = aiming;
    }

    public void PlayCockingAnimation()
    {
        // Not enabled yet!! (BG:2022-09-06)
        if (cockingAnimation != null)
            cockingAnimation.SetTrigger("Cock");
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
