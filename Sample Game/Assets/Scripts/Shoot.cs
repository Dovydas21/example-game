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

    Transform gun;
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

        gunPosition = gun.localPosition;
        gunRotation = gun.localEulerAngles;

        aimPosition = new Vector3(0f, -0.58f, 0.85f);
        aimRotation = new Vector3(0f, 180f, 0f);
    }
    public void Fire()
    {
        RaycastHit HitInfo;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out HitInfo, Mathf.Infinity))
        {
            Transform objectHit = HitInfo.transform;
            Debug.DrawLine(cam.transform.position, HitInfo.point, Color.red, 2, false);
            if (HitInfo.rigidbody != null) objectHit.GetComponent<Rigidbody>().AddForceAtPosition(cam.transform.forward * 1000, HitInfo.point, ForceMode.Impulse);
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