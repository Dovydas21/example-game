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
    private SFXScript sfx;
    private bool canShoot;
    public bool fullAuto;
    WaitForSeconds rapidFireWait;
    [SerializeField] private float fireRate = 0.5f;
    private float _nextRate =-1f;
    bool aiming = false;
    Vector3 gunPosition;
    Vector3 gunRotation;

    Vector3 aimPosition;
    Vector3 aimRotation;

    public void Start()
    {
        playerInput = new PlayerInput();
        onFoot = playerInput.OnFoot;
        gun = cam.GetComponent<Transform>().GetChild(0);
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
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out HitInfo, Mathf.Infinity))
        {
            Transform objectHit = HitInfo.transform;
            sfx.PlayShot();
            Debug.DrawLine(cam.transform.position, HitInfo.point, Color.red, 2, false);
            if (HitInfo.rigidbody != null) objectHit.GetComponent<Rigidbody>().AddForceAtPosition(cam.transform.forward * 1000, HitInfo.point, ForceMode.Impulse);
        }
       
    }

    public IEnumerator FullAuto()
    {
        if (CanShoot())
        {
            Fire();
            if (fullAuto)
            {
                while (CanShoot())
                {
                    yield return rapidFireWait;
                    Fire();
                }
            }
        }
  
    }

    bool CanShoot()
    {
        if (Time.time > _nextRate)
        {
            _nextRate = fireRate + Time.time;
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
            cam.fieldOfView = 60f;
            // Hard coded position of the gun when aiming down sights.

            gun.localPosition = aimPosition;
            gun.localEulerAngles = aimRotation;
        }
        else
        {
            aiming = false;
            cam.fieldOfView = 90f;
            gun.localPosition = gunPosition;
            gun.localEulerAngles = gunRotation;
        }
    }
}