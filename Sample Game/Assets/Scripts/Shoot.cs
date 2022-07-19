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

    Transform gun;
    public GameObject gunObject;
    private SFXScript sfx;
    private bool canShoot;
    WaitForSeconds rapidFireWait;
    [SerializeField] private float fireRate = 0.5f;
    private float _nextRate = -1f;
    bool aiming = false;
    GunInfo gunInfo;

    Vector3 gunPosition;
    Vector3 gunRotation;
    Vector3 aimPosition;
    Vector3 aimRotation;

    public void Start()
    {
        Refresh();
    }


    public void Refresh()
    {
        playerInput = new PlayerInput();
        onFoot = playerInput.OnFoot;

        gun = cam.GetComponent<Transform>().GetChild(0);
        if (gun == null)
            gunInfo = null;
        else
            gunInfo = gun.GetComponentInChildren<GunInfo>();

        sfx = GetComponent<SFXScript>();
        rapidFireWait = new WaitForSeconds(1f);

        gunPosition = gun.localPosition;
        gunRotation = gun.localEulerAngles;

        aimPosition = new Vector3(0f, -0.58f, 0.85f);
        aimRotation = new Vector3(0f, 180f, 0f);
    }

    public void Fire()
    {
        //Debug.Log("Fire Tigger Held? :" + test);

        RaycastHit HitInfo;

        gunInfo.PlayMuzzleFlash();

        gunInfo.PlayShootAnimation();
        gunInfo.PlayShootSound();

        gunInfo.PlayCockingAnimation();
        // gunInfo.PlayCockingSound();

        // sfx.PlayShot();
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out HitInfo, gunInfo.range))
        {
            Transform objectHit = HitInfo.transform;
            Debug.DrawLine(cam.transform.position, HitInfo.point, Color.red, 2, false);
            if (HitInfo.rigidbody != null)
                objectHit.GetComponent<Rigidbody>().AddForceAtPosition(cam.transform.forward * gunInfo.power, HitInfo.point, ForceMode.Impulse);
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
        if (Time.time > _nextRate)
        {
            _nextRate = gunInfo.fireRate + Time.time;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Aim()
    {
        if (!aiming)
        {
            aiming = true;
            cam.fieldOfView = gunInfo.aimedInFOV;
            gun.localPosition = gunInfo.aimedInPosition;
            gun.localEulerAngles = gunInfo.aimedInAngle;
        }
        else
        {
            aiming = false;
            cam.fieldOfView = 90f;
            gun.localPosition = gunInfo.defaultGunPosition;
            gun.localEulerAngles = gunInfo.defaultGunAngles;
        }
    }
}