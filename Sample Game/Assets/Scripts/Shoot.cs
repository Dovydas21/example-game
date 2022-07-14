using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shoot : MonoBehaviour
{
    public Camera cam;

    private PlayerInput playerInput;
    private PlayerInput.OnFootActions onFoot;

    public void Start()
    {

        playerInput = new PlayerInput();
        onFoot = playerInput.OnFoot;
    }

    public void Fire()
    {
        print("Fired!");
        RaycastHit HitInfo;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out HitInfo, 100.0f))
        {
            Transform objectHit = HitInfo.transform;
            objectHit.GetComponent<Rigidbody>().AddForceAtPosition(HitInfo.point, HitInfo.point, ForceMode.Impulse);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }
}