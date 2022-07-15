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

    public void Start()
    {
        playerInput = new PlayerInput();
        onFoot = playerInput.OnFoot;
        gun = cam.GetComponent<Transform>().GetChild(0);
        sfx = GetComponent<SFXScript>();
    }
    public void Fire()
    {
        canShoot = sfx.PlayShot();
        if (canShoot)
        {   
            RaycastHit HitInfo;
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out HitInfo, Mathf.Infinity))
            {
                Transform objectHit = HitInfo.transform;
                Debug.DrawLine(cam.transform.position, HitInfo.point, Color.red, 2, false);
                if (HitInfo.rigidbody != null) objectHit.GetComponent<Rigidbody>().AddForceAtPosition(cam.transform.forward * 1000, HitInfo.point, ForceMode.Impulse);
            }
        }
    }

    public void Aim()
    {
        // Hard coded position of the gun when aiming down sights.
        gun.localPosition = new Vector3(0f, -0.58f, 0.85f);
        gun.localEulerAngles = new Vector3(0f, 180f, 0f);
    }

    void FixedUpdate()
    {

    }
}