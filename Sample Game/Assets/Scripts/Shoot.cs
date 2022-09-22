using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shoot : MonoBehaviour
{
    public Camera cam;
    private RaycastHit Hitinfo;

    Transform gunHolder;
    Transform gun;
    public GameObject gunObject;
    private bool canShoot;              // Set to true while the character is able to shoot. See "CanShoot()" function.
    bool waitingForNextShot;            // Set to true while the character is waiting for the next round to be chambered.
    WaitForSeconds rapidFireWait;
    private float _nextRate = -1f;
    bool aiming = false;
    bool playerHoldingGun = false;
    GunInfo gunInfo;

    Vector3 defaultGunHolderPos;
    Vector3 defaultGunHolderRot;

    [Header("Key bindings")]
    public KeyCode FireKey;
    public KeyCode AimKey;
    public KeyCode ReloadKey;

    public void Start()
    {
        Refresh();
    }

    private void Update()
    {

        canShoot = CanShoot();

        // Shooting
        if (gunInfo.fullAuto)
        {
            if (Input.GetKey(FireKey) && canShoot)
                StartCoroutine(Fire());
        }
        else
        {
            if (Input.GetKeyDown(FireKey) && canShoot)
                StartCoroutine(Fire());
        }

        // Reloading
        if (Input.GetKey(ReloadKey))
            Reload();

        // Aiming
        if (Input.GetKeyDown(AimKey) || Input.GetKeyUp(AimKey))
            Aim();
    }

    public void Refresh()
    {

        gunHolder = GameObject.FindGameObjectWithTag("GunHolder").transform;

        if (gunHolder.childCount == 0) // Check that you have the 
        {
            gunInfo = null;
            playerHoldingGun = false;
        }
        else
        {
            gun = gunHolder.GetChild(0);
            playerHoldingGun = true;
            gunInfo = gunHolder.GetComponentInChildren<GunInfo>();
            rapidFireWait = new WaitForSeconds(gunInfo.fireRate);
        }
    }

    public IEnumerator Fire()
    {
        if (playerHoldingGun)
        {
            print("Shot fired");
            RaycastHit HitInfo;

            gunInfo.PlayMuzzleFlash();
            gunInfo.PlayShootAnimation();
            gunInfo.PlayShootSound();
            gunInfo.PlayCockingAnimation();

            gunInfo.UpdateAmmoInGun(gunInfo.ammoInGun - 1); // Reduce the current ammo count by 1.

            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out HitInfo, gunInfo.range))
            {
                Transform objectHit = HitInfo.transform;
                Debug.DrawLine(cam.transform.position, HitInfo.point, Color.red, 2, false);
                if (HitInfo.rigidbody != null)
                {
                    objectHit.GetComponent<Rigidbody>().AddForceAtPosition(cam.transform.forward * gunInfo.power, HitInfo.point, ForceMode.Impulse);
                }

                if (objectHit.GetComponent<EnemyController>() != null)
                {
                    print("Enemy hit!");
                    EnemyController enemyController = objectHit.GetComponent<EnemyController>();
                    enemyController.TakeDamage(gunInfo.damage);
                    enemyController.BleedAtPosition(HitInfo.point);
                }
            }
            waitingForNextShot = true;
            yield return new WaitForSeconds(gunInfo.fireRate); // Wait until you can fire the next round.
            waitingForNextShot = false;
        }
    }

    public void Drop()
    {
        StartCoroutine(gunInfo.Drop());
    }

    public IEnumerator FullAuto()
    {
        if (CanShoot())
        {
            Fire();
            if (gunInfo.fullAuto)
            {
                while (CanShoot())
                {
                    yield return rapidFireWait;
                    Fire();
                }
            }
        }

    }

    public bool CanShoot()
    {
        print("Shoot.cs:CanShoot(): Checking if the player is able to shoot the gun.");
        if (!playerHoldingGun)
        {
            print("Shoot.cs:CanShoot(): Player is not holding a gun and therefore cannot fire.");
            print("Shoot.cs:CanShoot(): Returning FALSE");
            return false; // Exit if the player is not holding a gun.
        }

        if (gunInfo.ammoInGun > 0 && !waitingForNextShot)
        {
            print("Shoot.cs:CanShoot(): Player has " + gunInfo.ammoInGun + " ammo in " + gunInfo.name + " and is not waiting for the next shot to be ready.");
            print("Shoot.cs:CanShoot(): Returning TRUE");
            return true;
        }
        else {
            print("Shoot.cs:CanShoot(): Player has " + gunInfo.ammoInGun + " ammo in " + gunInfo.name + " and 'waitingForNextShot' is set to " + waitingForNextShot);
            print("Shoot.cs:CanShoot(): Returning FALSE");
            return false;
        }
    }

    public void Aim()
    {
        if (!aiming) // If the player is not aiming in then set aiming to true.
            aiming = true;
        else
            aiming = false;

        if (playerHoldingGun) // Check that the player is actually holding a gun.
            gunInfo.ToggleAim(aiming); // Go and toggle aiming down sights.
    }


    public void Reload()
    {
        gunInfo.UpdateAmmoInGun(gunInfo.magCapacity);
    }

}