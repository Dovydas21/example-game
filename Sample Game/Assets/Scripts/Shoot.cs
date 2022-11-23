using System.Collections;
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
    public Transform ADSPosition; // Trasnform of the GameObject called "Aiming Position" so we can slerp to it to ADS.
    public GameObject gunObject;
    public GameObject bulletHoleDecal;
    public TrailRenderer bulletTrail;
    WaitForSeconds rapidFireWait;
    //     [SerializeField] private float fireRate = 0.5f;
    public bool firing = false;
    public bool playerHoldingGun = false;
    bool aiming = false;
    GunInfo gunInfo;
    List<Vector3> hitPositions = new List<Vector3>();

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
    }

    public void Fire()
    {
        if (playerHoldingGun)
        {
            firing = true;
            print("Shot fired");
            RaycastHit HitInfo;
            // gunHolder.GetComponent<GunRecoil>().Recoil(); // Call the recoil function.
            gunHolder.GetComponent<WeaponSway>().Recoil(); // Call the recoil function.

            gunInfo.PlayMuzzleFlash();
            gunInfo.PlayShootSound();
            gunInfo.PlayCockingAnimation();

            gunInfo.UpdateAmmoInGun(gunInfo.ammoInGun - 1); // Reduce the current ammo count by 1.

            if (Physics.Raycast(gunInfo.gunObj.transform.position, gunInfo.gunObj.transform.forward, out HitInfo, gunInfo.range))
            {
                Transform objectHit = HitInfo.transform;
                hitPositions.Add(HitInfo.point);
                Debug.DrawLine(cam.transform.position, HitInfo.point, Color.red, 20f, false);

                TrailRenderer trail = Instantiate(bulletTrail, gunHolder.transform.position, Quaternion.identity);
                StartCoroutine(BulletTrail(HitInfo, trail));

                if (objectHit.transform.gameObject.tag != "Player")
                {
                    // Spawn a bullethole decal.
                    GameObject bulletHole = Instantiate(bulletHoleDecal, HitInfo.point + HitInfo.normal * .001f, Quaternion.identity); // Spawn the bullethole decal.
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
                        enemyController.TakeDamage(gunInfo.damage);
                        enemyController.BleedAtPosition(HitInfo.point);
                    }
                }
            }
            firing = false;
        }
    }

    private IEnumerator BulletTrail(RaycastHit hitInfo, TrailRenderer trail)
    {
        float t = Time.time;
        Vector3 originalPos = trail.transform.position;

        while(t < 1)
        {
            trail.transform.position = Vector3.Lerp(originalPos, hitInfo.point, t);
            t += Time.deltaTime / trail.time;
            yield return new WaitForSeconds(.01f);
        }

        Destroy(trail, trail.time);
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

    private void OnDrawGizmos()
    {
        foreach (var hitPos in hitPositions)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(hitPos, new Vector3(.1f, .1f, .1f));
            Gizmos.DrawLine(gunHolder.transform.position, hitPos);
        }
    }

    public void Reload()
    {
        gunInfo.UpdateAmmoInGun(gunInfo.magCapacity);
    }

}