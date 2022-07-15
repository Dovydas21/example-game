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
    private SFXScript sfx;
    private bool canShoot;

    public void Start()
    {

        playerInput = new PlayerInput();
        onFoot = playerInput.OnFoot;
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

    void FixedUpdate()
    {

    }
}