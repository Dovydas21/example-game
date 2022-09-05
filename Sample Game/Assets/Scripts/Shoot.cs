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
    private SFXScript sfx;
    private bool canShoot;
    WaitForSeconds rapidFireWait;
    [SerializeField] private float fireRate = 0.5f;
    private float _nextRate = -1f;
    bool aiming = false;
    bool playerHoldingGun = false;
    GunInfo gunInfo;

    Vector3 defaultGunHolderPos;
    Vector3 defaultGunHolderRot;

    [Header("Key bindings")]
    public KeyCode FireKey;
    public KeyCode AimKey;

    public void Start()
    {
        Refresh();
    }

    private void Update()
    {
        if (Input.GetKey(FireKey))
            Fire();

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

        sfx = GetComponent<SFXScript>();
    }

    public void Fire()
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

            // sfx.PlayShot();
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
        if (!playerHoldingGun) return false; // Exit if the player is not holding a gun.

        if (gunInfo.ammoInGun > 0)
            return true;
        else return false;

        // print("Time.time = " + Time.time + " " + "_nextRate = " + _nextRate);
        //if (Time.time > _nextRate && gunInfo.ammoInGun > 0)
        //{
        //    _nextRate = gunInfo.fireRate + Time.time;
        //    return true;
        //}
        //return true;
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