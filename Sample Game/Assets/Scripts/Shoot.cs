using System.Collections;
using System.Collections.Generic;
using System.Threading;
//using UnityEditor.PackageManager;
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
    [SerializeField] private GunRecoil gunRecoilScript;
    [SerializeField] private CameraRecoil camRecoilScript;
    public Transform ADSPosition;               // Trasnform of the GameObject called "Aiming Position" so we can slerp to it to ADS.
    public GameObject bulletHoleDecal;
    WaitForSeconds rapidFireWait;
    public bool firing = false;
    public bool playerHoldingGun = false;
    public bool playerSummonedWeapon = false;
    float nextShotTime;                         // The time that the next shot is allowed to be fired at or after.
    GunInfo gunInfo;
    List<Vector3> hitPositions = new List<Vector3>();

    public void Start()
    {
        Refresh();
        nextShotTime = Time.time;
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
    }

    public void Fire()
    {
        if (playerHoldingGun && Time.time >= nextShotTime && Time.timeScale > 0f)
        {
            nextShotTime = Time.time + gunInfo.fireRate;
            firing = true;
            print("Shot fired");
            RaycastHit HitInfo;

            gunRecoilScript.RecoilFire();
            camRecoilScript.RecoilFire();

            gunInfo.PlayMuzzleFlash();
            gunInfo.PlayShootSound();
           

            // Set the bullet offset to Vector3.Zero for the first shot, this way any guns with 1 projectile will fire straight and the first bullet will always be on target.
            Vector3 nextBulletOffset = Vector3.zero;

            gunInfo.UpdateAmmoInGun(gunInfo.ammoInGun - gunInfo.ammoReductionPerShot); // Reduce the current ammo count.

            // Loop through and fire a shot for as many projectiles as are defined in GunInfo.cs for the gun the player is holding.
            for (int i = 0; i < gunInfo.projectileCount; i++)
            {
                if (i != 0) // If we have fired the first bullet, the next projectile should have its direction randomised by the "gunInfo.projectileSpread" value.
                {
                    nextBulletOffset += new Vector3(Random.Range(-gunInfo.projectileSpread, gunInfo.projectileSpread), Random.Range(-gunInfo.projectileSpread, gunInfo.projectileSpread), Random.Range(-gunInfo.projectileSpread, gunInfo.projectileSpread));
                }

                bool shotHit = Physics.Raycast(gunInfo.bulletOrigin.position, gunInfo.bulletOrigin.forward + nextBulletOffset, out HitInfo, gunInfo.range);

                if (shotHit)
                {
                    Transform objectHit = HitInfo.transform;
                    hitPositions.Add(HitInfo.point);
                    Debug.DrawLine(cam.transform.position, HitInfo.point, Color.red, 20f, false);

                    if (objectHit.transform.gameObject.tag != "Player")
                    {
                        // Spawn a bullethole decal.
                        GameObject bulletHole = Instantiate(bulletHoleDecal, HitInfo.point + HitInfo.normal * .001f, Quaternion.identity); // Spawn the bullethole decal.
                        // \/ VERY LAGGY!! \/
                        if (gunInfo.blackHoleEffect && i == 0)
                            bulletHole.AddComponent<BlackHole>();
                        // /\ VERY LAGGY!! /\
                        bulletHole.transform.localScale = new Vector3(.1f, .1f, .1f); // Set the scale of the decal.
                        bulletHole.transform.parent = objectHit; // Parent the decal to the object that was hit.
                        bulletHole.transform.LookAt(HitInfo.point + HitInfo.normal); // Reposition the decal to be oriented on the surface of the hit object.
                        Destroy(bulletHole, 10f); // Destroy the decal after 10 seconds...

                        if (HitInfo.rigidbody != null)
                            objectHit.GetComponent<Rigidbody>().AddForceAtPosition(cam.transform.forward * gunInfo.power, HitInfo.point, ForceMode.Impulse);

                        if (objectHit.GetComponent<EnemyController>() != null)
                        {
                            print("Enemy hit!");
                            EnemyController enemyController = objectHit.GetComponent<EnemyController>();
                            enemyController.BleedAtPosition(HitInfo.point);
                            enemyController.TakeDamage(gunInfo.damage);

                            if (gunInfo.freezeEffect && gunInfo.freezeDuration > 0f) // If the gun has a freeze effect, freeze the enemy that was hit.
                            {
                                StartCoroutine(enemyController.FreezeEnemy(gunInfo.freezeDuration));
                            }
                        }
                    }
                }


                if (gunInfo.bulletTrail != null && gunInfo.bulletTrailSpeed > 0f) // Ensure that we have a bullet trail defined before spawning one in.
                {
                    TrailRenderer trail = Instantiate(gunInfo.bulletTrail, gunInfo.muzzleFlash.transform.position, Quaternion.identity);
                    Vector3 bulletDestination;
                    if (shotHit)
                    {
                        bulletDestination = HitInfo.point;
                    }
                    else
                    {
                        bulletDestination = new Vector3(gunInfo.muzzleFlash.transform.forward.x, gunInfo.muzzleFlash.transform.forward.y, gunInfo.muzzleFlash.transform.forward.z + 1000f);
                    }
                    StartCoroutine(BulletTrail(bulletDestination, trail));
                    Destroy(trail, 1f);
                }

            }

            firing = false;
        }
    }

    IEnumerator BulletTrail(Vector3 destination, TrailRenderer trail)
    {
        float t = 0;
        float targetTime = t + 1;
        Vector3 originalPos = gun.GetComponent<GunInfo>().muzzleFlash.transform.position;

        while (t < targetTime)
        {
            trail.transform.position = Vector3.Lerp(originalPos, destination, t);
            t += Time.deltaTime * gunInfo.bulletTrailSpeed;
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }

    // Drop the weapon.
    public void Drop()
    {
        StartCoroutine(gunInfo.Drop());
    }


    // Fire in a loop waiting for the appropriate amount of time between shots.
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
                    yield return new WaitForEndOfFrame();
                    Fire();
                }
            }
        }

    }

    public bool CanShoot()
    {
        if (!playerHoldingGun)
            return false; // Exit if the player is not holding a gun.

        if (gunInfo.ammoInGun > 0) // Check that we still have bullets in the gun.
            return true;
        else
            return false;
    }

    public void Reload()
    {
        StartCoroutine(gunInfo.ReloadSpin()); // Play the reload spin.
    }
}