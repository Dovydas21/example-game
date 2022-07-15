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
    public void Start()
    {

        playerInput = new PlayerInput();
        onFoot = playerInput.OnFoot;
    }
    public void Fire()
    {
        /*
        RaycastHit HitInfo;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out HitInfo, 100.0f))
        {
            Transform objectHit = HitInfo.transform;
            Debug.Log("Transform pos"+ this.transform.position);
            Debug.Log("End Point" + HitInfo.point);
            Debug.DrawLine(this.transform.position, this.transform.forward, Color.red, 2, false);
            if (HitInfo.rigidbody != null)

                objectHit.GetComponent<Rigidbody>().AddForceAtPosition(this.transform.position, HitInfo.point, ForceMode.Impulse);
            
        }
        */

        RaycastHit HitInfo;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out HitInfo, 100.0f))
        {
            Transform objectHit = HitInfo.transform;
            Debug.DrawLine(cam.transform.position, HitInfo.point, Color.red, 2, false);
            if (HitInfo.rigidbody != null)
                
            objectHit.GetComponent<Rigidbody>().AddForceAtPosition(cam.transform.forward * 1000, HitInfo.point, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {

    }
}