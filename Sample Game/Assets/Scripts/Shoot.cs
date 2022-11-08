﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shoot : MonoBehaviour
{
    public Camera cam;
    private PlayerInput playerInput;
    private PlayerInput.OnFootActions onFoot;
    private RaycastHit Hitinfo;

    Transform gunHolder;
    Transform gun;
    public GameObject gunObject;
    public GameObject bulletHoleDecal;
    private SFXScript sfx;
    private bool canShoot;
    WaitForSeconds rapidFireWait;
    [SerializeField] private float fireRate = 0.5f;
    private float _nextRate = -1f;
    public bool firing = false;
    bool aiming = false;
    bool playerHoldingGun = false;
    GunInfo gunInfo;
    

    Vector3 defaultGunHolderPos;
    Vector3 defaultGunHolderRot;

    public void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        playerInput = new PlayerInput();
        onFoot = playerInput.OnFoot;

        gunHolder = GameObject.FindGameObjectWithTag("GunHolder").transform;

        if (gunHolder.childCount == 0) // Check that you have the 
        {
            gunInfo = null;
            gameObject.GetComponent<InputManager>().gunInfo = null; // Set the gunInfo component to be null in input manager as well.
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
            firing = true;
            print("Shot fired");
            RaycastHit HitInfo;
            gunHolder.GetComponent<GunRecoil>().Recoil(); // Call the recoil function.

            gunInfo.PlayMuzzleFlash();
            gunInfo.PlayShootSound();
            gunInfo.PlayCockingAnimation();

            gunInfo.UpdateAmmoInGun(gunInfo.ammoInGun - 1); // Reduce the current ammo count by 1.

            // sfx.PlayShot();
            if (Physics.Raycast(gunInfo.gunObj.transform.position, gunInfo.gunObj.transform.forward, out HitInfo, gunInfo.range))
            {
                Transform objectHit = HitInfo.transform;
                Debug.DrawLine(cam.transform.position, HitInfo.point, Color.red, 2, false);
                GameObject bulletHole = Instantiate(bulletHoleDecal, objectHit, true);
                if (HitInfo.rigidbody != null)
                    objectHit.GetComponent<Rigidbody>().AddForceAtPosition(cam.transform.forward * gunInfo.power, HitInfo.point, ForceMode.Impulse);

                if (objectHit.GetComponent<EnemyController>() != null)
                {
                    print("Enemy hit!");
                    EnemyController enemyController = objectHit.GetComponent<EnemyController>();
                    enemyController.TakeDamage(gunInfo.damage);
                    enemyController.BleedAtPosition(HitInfo.point);
                }
            }
            firing = false;
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