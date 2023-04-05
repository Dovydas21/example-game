using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    // Variables for gun sway:
    [Header("Sway variables:")]
    public float swayMultiplier;
    public float swaySmooth;
    Quaternion targetRotation;

    // Variables for gun recoil:
    [Header("Recoil variables:")]
    Vector3 currentRotation;
    Vector3 recoilRotation;
    Vector3 targetPosition;
    Vector3 currentPosition;
    Vector3 initialGunPosition;
    public Camera cam;
    [Tooltip("Side to side recoil.")][SerializeField] float recoilX;                                 // Side to side recoil
    [Tooltip("Up and down recoil.")][SerializeField] float recoilY;                                  // Up and down recoil
    [Tooltip("Front and back recoil.")][SerializeField] float recoilZ;                               // Front and back recoil
    [Tooltip("Amount to move the gun backwards with each shot.")][SerializeField] float kickbackZ;   // Amount to move the gun backwards with each shot
    [Tooltip("Speed / power of the recoil")] public float snappiness;                                // How snappy the recoil feels.                            
    [Tooltip("Speed to return the gun to the original position.")] public float returnAmount;        // The speed at which the weapon returns to it's original pos

    // Variables for gun aiming
    [Header("Aiming variables:")]
    public Transform aimPos;            // The Transform component of the "AimingPosition" GameObject.
    public Shoot shoot;                 // A reference to the Shoot.cs script.
    public KeyCode aimKey;              // The key the player presses to aim down sights.
    public float aimSpeed = .5f;        // The speed that the gun moves to the "AimingPosition".
    float initialFOV;                   // The FOV of the camera by default.
    bool playerAiming = false;          // Boolean value to track whether the player is aiming in or not.
    float startTime;                    // A value used to smoothly calculate the movement of aiming into the sights of the gun.      
    float journeyLength;                // A value used to smoothly calculate the movement of aiming into the sights of the gun.
    bool playerTriggeredAim = false;
    float keyDownTime, keyUpTime;
    public float aimSmooth;
    float initialAimSmooth;

    // References
    GunInfo gunInfo;

    // Variables for debugging.
    Vector3 debugPosition;

    private void Start()
    {
        initialGunPosition = transform.localPosition;
        startTime = Time.time;
        journeyLength = Vector3.Distance(transform.position, aimPos.position);
        initialFOV = cam.fieldOfView;
        initialAimSmooth = swaySmooth;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.transform.childCount > 0)
        {
            gunInfo = gameObject.transform.GetComponentInChildren<GunInfo>();
            Quaternion gunHolderRot = CalculateSway() * RecoilRotation();
            Vector3 gunHolderPos = Kickback();
            Quaternion camRot = RecoilRotation();


            if (shoot.playerHoldingGun) // If the player is holding a gun.
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

                // Position values for the gunHolder based on whether we are aiming or not.
                Vector3 aim = Aim(playerTriggeredAim); // Work out the position of the gun if we're currently scoping in or out.
                Vector3 positionDifference = targetPosition - aim;
                gunHolderPos -= positionDifference;
            }

            debugPosition = aimPos.position;
            transform.localRotation = gunHolderRot;
            cam.transform.rotation = camRot;
            transform.localPosition = gunHolderPos;
        }
    }

    Vector3 Aim(bool aim)
    {
        float distCovered = (Time.time - startTime) * aimSpeed; // Distance moved equals elapsed time times speed..
        float fractionOfJourney = distCovered / journeyLength; // Fraction of journey completed equals current distance divided by total distance.
        Vector3 result;

        if (aim && gunInfo.canAim)
        {
            result = Vector3.Slerp(initialGunPosition, aimPos.localPosition, fractionOfJourney);
            cam.fieldOfView = Mathf.LerpAngle(initialFOV, gunInfo.aimFieldOfView, fractionOfJourney);
            aimSmooth = initialAimSmooth / 3f;
        }
        else
        {
            result = Vector3.Slerp(aimPos.localPosition, initialGunPosition, fractionOfJourney);
            cam.fieldOfView = Mathf.LerpAngle(gunInfo.aimFieldOfView, initialFOV, fractionOfJourney);
            aimSmooth = initialAimSmooth;
        }
        return result;
    }

    Quaternion CalculateSway()
    {
        // Get mouse input.
        float mouseX = Input.GetAxisRaw("Mouse X") * swayMultiplier;
        float mouseY = Input.GetAxisRaw("Mouse Y") * swayMultiplier;

        // Work out the rotation.
        Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(-mouseX, Vector3.up);

        // Work out where we are ultimately moving the gun to.
        targetRotation = rotationX * rotationY;
        Quaternion swayAngle = Quaternion.Lerp(transform.localRotation, targetRotation, swaySmooth * Time.deltaTime);

        return swayAngle;
    }

    Quaternion RecoilRotation()
    {
        recoilRotation = Vector3.Lerp(recoilRotation, Vector3.zero, Time.deltaTime * returnAmount);
        currentRotation = Vector3.Slerp(currentRotation, recoilRotation, snappiness * Time.deltaTime);
        print("currentRotation = " + currentRotation + ", recoilRotation = " + recoilRotation);
        return Quaternion.Euler(currentRotation);
    }

    public void Recoil() // Called from shoot.cs when the player shoots.
    {
        Vector3 kickBack = new Vector3(0f, 0f, kickbackZ);
        Vector3 recoil = new Vector3(recoilX, Random.Range(0f, recoilY), Random.Range(-recoilZ, recoilZ));
        kickBack *= -1f;
        recoil *= -1f;

        recoilRotation += new Vector3(recoilX * 10f, recoilY * 10f, recoilZ * 10f);

        // Add these to the target position of the GunHolder object.
        targetPosition -= kickBack;
        targetPosition += recoil;
    }

    Vector3 Kickback()
    {
        targetPosition = Vector3.Lerp(targetPosition, initialGunPosition, Time.deltaTime * returnAmount); // Moves from the gun's last target position back to the initial gun position.
        currentPosition = Vector3.Lerp(currentPosition, targetPosition, Time.fixedDeltaTime * snappiness); // Moves the gun from the current position to the target position.
        return currentPosition; // Unless aiming or firing, 'currentPosition' will be equal to the initialGunPosition.
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawSphere(debugPosition, .1f);
    }
}