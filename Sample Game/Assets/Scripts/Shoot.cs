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
    Vector3 defaultADSPos;
    List<Vector3> hitPositions = new List<Vector3>();

    public void Start()
    {
        Refresh();
        defaultGunHolderPos = gunHolder.localPosition;
        defaultADSPos = ADSPosition.localPosition;
    }

    private void Update()
    {

        Aim(Input.GetKey(KeyCode.Mouse1));
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

            if (Physics.Raycast(gunInfo.gunObj.transform.position, gunInfo.gunObj.transform.forward, out HitInfo, gunInfo.range))
            {
                Transform objectHit = HitInfo.transform;
                hitPositions.Add(HitInfo.point);
                Debug.DrawLine(cam.transform.position, HitInfo.point, Color.red, 20f, false);

                if (objectHit.transform.gameObject.tag != "Player")
                {
                    // Spawn a bullethole decal.
                    GameObject bulletHole = Instantiate(bulletHoleDecal, HitInfo.point + HitInfo.normal * .001f, Quaternion.identity); // Spawn the bullethole decal.
                    bulletHole.transform.localScale = new Vector3(.1f, .1f, .1f); // Set the scale of the decal.
                    bulletHole.transform.parent = objectHit; // Parent the decal to the object that was hit.
                    bulletHole.transform.LookAt(HitInfo.point + HitInfo.normal); // Reposition the decal to be oriented on the surface of the hit object.
                    Destroy(bulletHole, 10f); // Destroy the decal after 10 seconds...


                    Debug.DrawLine(HitInfo.point, HitInfo.normal, Color.green, 20f, false);

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

    public void Aim(bool aim)
    {
        if (playerHoldingGun)
        {
            //if (!aiming)
            if(aim)
            {  // If the player is not aiming in then set aiming to true.
                print("Player now aiming down sights, moving gun to ADS position.");
                aiming = true;
                Vector3 targetPosition = ADSPosition.position;
                Vector3 slerpPos = Vector3.Slerp(defaultGunHolderPos, targetPosition, Time.deltaTime * 10f);
                print("slerpPos = " + slerpPos);
                gunHolder.localPosition = slerpPos;
            }
            else
            {
                print("Player no longer aiming down sights, moving gun to gunHolder position.");
                aiming = false;
                Vector3 targetPosition = defaultGunHolderPos;
                Vector3 slerpPos = Vector3.Slerp(defaultADSPos, targetPosition, Time.deltaTime * 10f);
                print("slerpPos = " + slerpPos);
                gunHolder.localPosition = slerpPos;
            }
        }

        /*
        if (playerHoldingGun) // Check that the player is actually holding a gun.
            gunInfo.ToggleAim(aiming); // Go and toggle aiming down sights.
        */
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