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
    public bool canAim;                                         // True = Gun can be moved back and forth from the "AimingPosition".
    public bool hasToBeCocked;                                  // True = Gun must be cocked (e.g. Shotgun / Revolver), False = Gun is semi-auto.

    [Header("Gun stats")]
    public float power;                                         // The force multiplier applied when the gun lands a hit on a RigidBody.
    public float range;                                         // The range of the raycast shot when the gun is fired.
    public float fireRate;                                      // The fire rate of the gun. .1 is fast 10 is slow.
    public int magCapacity;                                     // The gun's capacity of rounds that it can fire before reloading.
    public int damage;                                          // Damage the gun does when it hits enemies once.
    public int projectileCount = 1;                             // The number of projectiles fired from the weapon at one time, can be used for shotguns, burst fire etc.
    public int ammoInGun;                                       // The amount of ammo currently inside of the gun.

    [Header("Gun positions")]
    public GameObject gunHolder;                                // Gameobject attached to the camera that holds the gun.
    public Vector3 defaultGunPosition;                          // The local position of the gun inside of gunHolder when NOT aimed in.
    public Quaternion defaultGunAngles;                         // The local rotation of the gun inside of gunHolder when NOT aimed in.
    public Vector3 aimPosition;                                 // The local position of the "AimingPosition" GameObject for this weapon when we are aiming down sights.
    public float aimFieldOfView = 30f;                          // The Field Of View that the camera will change to when player aims in.
    bool playerAimedDownSights = false;                         // Keeps track of whether the player is currently aiming down their sights.

    [Header("Gun animations")]
    public Animator shootingAnimation;                          // The animaTOR that has the animation the gun should play when the gun is fired.
    public Animator cockingAnimation;                           // The animaTOR that has the animation the gun should play when the gun is being cocked.
    public ParticleSystem muzzleFlash;                          // The particle effect that should serve as the muzzle flash when the gun is fired.
    public Transform bulletOrigin;                              // Where the bullet originates. i.e. The end of the barrel.
    public TrailRenderer bulletTrail;                           // The TrailRenderer asset that immitates the travel of a tracer round, when the gun is fired.
    public float bulletTrailSpeed = 1000f;                      // The speed that the trail moves to the hitposition in Shoot.cs.

    [Header("Gun pickup & drop")]
    public float gunPickupSpeed = 100f;                         // The speed that the gun moves to the GunHolder position when the player picks it up.
    public float gunPickupDistance = 20f;                       // The maximum distance the player is allowed to be from gun.
    public KeyCode gunPickupKey;                                // The key the player presses to pickup / summon the weapon.
    public float throwForce = 25f;                              // The force applied to the weapon when the player presses the key to drop the weapon.

    [Header("Gun audio")]
    public AudioSource shotSoundEffect;                         // The audio source of the sound effect that should play when the gun is fired.
    public AudioSource cockingSoundEffect;                      // The audio source of the sound effect that should play when the gun is cocked.

    [Header("Misc components")]
    public BoxCollider pickupTrigger;                           // The box collider that triggers the player to pick up the gun if it is on the ground.
    public Shoot shootScript;
    public GunRecoil gunRecoilScript;
    public GameObject gunObj;
    GameObject playerObj;                                       // The "Player" GameObject
    GameObject aimingPositionObj;                               // The "AimingPosition" GameObject
    float allowedToPickupTime;


    // Rigidbody variables.
    Rigidbody rb;
    float gunMass, gunDrag;
    bool gunGravity;

    private void Start()
    {
        ammoInGun = magCapacity; // Set the current ammo count to be the mag capacity (i.e. fully loaded).
        gunHolder = GameObject.FindGameObjectWithTag("GunHolder");
        aimingPositionObj = GameObject.FindGameObjectWithTag("AimingPosition");
        playerObj = GameObject.FindGameObjectWithTag("Player");
        shootScript = playerObj.GetComponent<Shoot>();
        rb = gameObject.GetComponent<Rigidbody>();
        allowedToPickupTime = Time.time;

        // Remember properties about the rigidbody of the object.
        gunMass = rb.mass;
        gunDrag = rb.drag;
        gunGravity = rb.useGravity;
    }

    private void OnMouseOver()
    {
        if (Vector3.Distance(transform.position, playerObj.transform.position) <= gunPickupDistance && Time.time >= allowedToPickupTime)
        {
            UIText.SetText(gameObject.name);
            if (Input.GetKey(gunPickupKey))
            {
                StartCoroutine(SummonWeapon());
            }
        }
    }

    private void OnMouseExit()
    {
        UIText.ClearText();
    }

    private void OnTriggerEnter(Collider pickupTrigger)
    {
        bool playerHandsEmpty = gunHolder.transform.childCount == 0;
        if (pickupTrigger.tag == "Player" && playerHandsEmpty && Time.time >= allowedToPickupTime) // Trigger a pick-up if you are the player, you are not already holding something and that you have waited long enough after dropping the gun the last time...
        {
            transform.parent = gunHolder.transform;
            transform.rotation = defaultGunAngles;
            gunHolder.transform.localPosition = defaultGunPosition;

            aimingPositionObj.transform.localPosition = aimPosition;

            //transform.localRotation = defaultGunAngles;
            //transform.localPosition = defaultGunPosition;

            playerObj.GetComponent<InputManager>().gunInfo = this;

            Destroy(gameObject.GetComponent<Rigidbody>());              // Disable the rigidbody on the object.
            gameObject.GetComponent<BoxCollider>().enabled = false;     // Disable the box collider that was used to trigger the pickup.

            shootScript.Refresh(); // Refresh the shoot script to give it information about the gun just picked up.
            UpdateAmmoInGun(ammoInGun);
        }
    }

    private IEnumerator SummonWeapon() // Called when the player presses the "gunPickupKey" when looking at the weapon.
    {
        bool playerHandsEmpty = gunHolder.transform.childCount == 0;
        if (playerHandsEmpty)
        {
            // Start keeping track of the time, t will be used to decide how far along slerp we are.
            float t = 0;

            // Remember the original gun's rotation and postition.
            Vector3 originalPos = transform.position;
            Quaternion originalRot = transform.rotation;

            // Calculate how far away the gun is from the player.
            float distanceFromPlayer = Vector3.Distance(originalPos, gunHolder.transform.position);

            // Work out the centre of the two points and move it down slightly so that the Slerp arcs UP instead of to the side.
            Vector3 centrePos = (originalPos + gunHolder.transform.position) / 2f;
            centrePos -= new Vector3(0, 1, 0);
            Vector3 gunRelCenter = transform.position - centrePos; // Work out the relative centre of the gun object.

            // While the gun is in transit...
            while (distanceFromPlayer > 0.1f && t <= 1)
            {
                Vector3 holderRelCenter = gunHolder.transform.position - centrePos; // Work out the relative centre of the gunHolder in the loop incase the player moves.
                transform.position = Vector3.Slerp(gunRelCenter, holderRelCenter, t) + centrePos; // Move the gun...
                transform.rotation = Quaternion.Slerp(originalRot, gunHolder.transform.rotation, t); // Rotate the gun...
                t += Time.deltaTime * gunPickupSpeed; // Increment t.
                yield return new WaitForEndOfFrame(); // Wait for the next frame
            }

            // Destroy the rigidbody from the object.
            Destroy(gameObject.GetComponent<Rigidbody>());

            // Fully set the rot and pos of the gun incase t was <1.
            transform.position = gunHolder.transform.position;
            transform.rotation = gunHolder.transform.rotation;
        }
        yield return null;
    }

    public IEnumerator Drop()
    {
        // Weapon will not drop when we have a value for "GunHolder".
        if (!playerAimedDownSights)
        {
            allowedToPickupTime = Time.time + .5f; // Set the time that the player is allowed to pickup the gun again after...
            gameObject.transform.parent = null;

            // Set the Rigidbody values back to what they were before we destroyed it.
            rb = gameObject.AddComponent<Rigidbody>();
            rb.mass = gunMass;
            rb.drag = gunDrag;
            rb.useGravity = gunGravity;

            rb.AddForce(transform.forward * throwForce, ForceMode.Impulse); // Throw the gun with some force rather than just letting it drop.
            rb.AddTorque(transform.right * throwForce, ForceMode.VelocityChange);
            ResetAmmoCounter(); // Hide the ammo counter.
            shootScript.Refresh(); // Refresh the shoot script to give it information about the gun just picked up / dropped.

            //yield return new WaitForSeconds(3f); // Wait for 1 second before re-enabling the pickup-trigger.

            gameObject.GetComponent<BoxCollider>().enabled = true; // Re-enable the pickup trigger.
        }
        yield return null;
    }

    public void ResetAmmoCounter()
    {
        GameObject.Find("AmmoCounter").GetComponent<Text>().text = "";
    }

    public void UpdateAmmoInGun(int value)
    {
        ammoInGun = value;
        GameObject.Find("AmmoCounter").GetComponent<Text>().text = ammoInGun.ToString();
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
