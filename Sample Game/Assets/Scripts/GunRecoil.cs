﻿using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;

public class GunRecoil : MonoBehaviour
{
    private Vector3 currentRotation;
    private Vector3 targetRotation;
    [SerializeField] private KeyCode aimKey = KeyCode.Mouse1;
    [SerializeField] private Camera cam;


    [SerializeField] private float recoilX, recoilY, recoilZ;
    [SerializeField] private float snappiness, returnSpeed, swayMultiplier, swaySmooth, AimSpeed;

    // Variables used to control aiming.
    [SerializeField] private Transform aimPos;
    private Vector3 initialGunPosition;
    private bool playerTriggeredAim = false, playerAiming = false;
    private float keyDownTime, keyUpTime, startTime, journeyLength;

    private void Start()
    {
        initialGunPosition = transform.localPosition;
        journeyLength = Vector3.Distance(transform.position, aimPos.position);
    }

    private void Update()
    {
        
        if (Input.GetKey(KeyCode.Mouse0)) RecoilFire();

        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.deltaTime);

        transform.localRotation = Quaternion.Euler(currentRotation);
        transform.localPosition = Aim(AimCheck());
    }

    public void RecoilFire()
    {
        //targetRotation += new Vector3(transform.position.x * .3f, transform.position.y * .3f, transform.position.z * .3f);
        targetRotation += new Vector3(-recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
    }

    bool AimCheck()
    {
        // Input controls for aiming.
        if (Input.GetKeyDown(aimKey)) // RMB held
        {
            playerTriggeredAim = !playerTriggeredAim; // Toggle the aiming to be the reverse of what it was before.
            keyDownTime = Time.time; // Remember when we pressed the button down...
        }
        else if (Input.GetKeyUp(aimKey))
        {
            keyUpTime = Time.time; // Remember when we released the button...
        }

        float keyUpDownDifference = keyUpTime - keyDownTime; // work out the difference between pressing and releasing the aimKey.
        if (keyUpDownDifference > 0 && keyUpDownDifference > .2f) // If the player has been holding the key down and they've just let go.
        {
            playerTriggeredAim = false;
        }
        else if (keyUpDownDifference < 0 && keyUpDownDifference > .2f) // If the player has just pressed the down key quickly then set aiming to true.
        {
            playerTriggeredAim = true;
        }

        // Work out if we are currently aiming in or not aiming in.
        if (Vector3.Distance(transform.localPosition, aimPos.localPosition) < 0.01f && !playerAiming && playerTriggeredAim) // Gun holder has reached the aiming position.
        {
            playerAiming = true;
            startTime = Time.time;
        }
        else if (Vector3.Distance(transform.localPosition, initialGunPosition) < 0.01f && playerAiming && !playerTriggeredAim) // Gun holder has reached the original pos again.
        {
            playerAiming = false;
            startTime = Time.time;
        }

        return playerTriggeredAim;
    }

    Vector3 Aim(bool aim)
    {
        float distCovered = (Time.time - startTime) * AimSpeed; // Distance moved equals elapsed time times speed..
        float fractionOfJourney = distCovered / journeyLength; // Fraction of journey completed equals current distance divided by total distance.
        Vector3 result;

        if (aim)
        {
            result = Vector3.Slerp(initialGunPosition, aimPos.localPosition, fractionOfJourney);
            cam.fieldOfView = Mathf.LerpAngle(90f, 30f, fractionOfJourney);
        }
        else
        {
            result = Vector3.Slerp(aimPos.localPosition, initialGunPosition, fractionOfJourney);
            cam.fieldOfView = Mathf.LerpAngle(30f, 90f, fractionOfJourney);
        }

        return result;
    }
}
