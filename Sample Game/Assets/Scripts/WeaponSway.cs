using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    // Variables for gun sway:
    [Header("Sway variables:")]
    public float swayMultiplier;
    public float smooth;
    Quaternion targetRotation;

    // Variables for gun recoil:
    [Header("Recoil variables:")]
    Vector3 currentRotation;
    Vector3 recoilRotation;
    Vector3 targetPosition;
    Vector3 currentPosition;
    Vector3 initialGunPosition;
    public Camera cam;
    [Tooltip("Side to side recoil.")]
    [SerializeField] float recoilX; // Side to side recoil
    [Tooltip("Up and down recoil.")]
    [SerializeField] float recoilY; // Up and down recoil
    [Tooltip("Front and back recoil.")]
    [SerializeField] float recoilZ; // Front and back recoil
    [Tooltip("Amount to move the gun backwards with each shot.")]
    [SerializeField] float kickbackZ; // Amount to move the gun backwards with each shot
    [Tooltip("Speed / power of the recoil")]
    public float snappiness;
    [Tooltip("Speed to return the gun to the original position.")]
    public float returnAmount;

    // Variables for gun aiming
    [Header("Aiming variables:")]
    public Transform aimPos;
    public Shoot shoot;
    public KeyCode aimKey;
    public Quaternion aimRot;
    public Quaternion originalRot;
    public float aimSpeed = .5f;
    public float ADSFov = 30f;
    float initialFOV; // The FOV of the camera by default.
    bool playerAiming = false;
    float startTime;
    float journeyLength;
    Vector3 debugPosition;

    private void Start()
    {
        initialGunPosition = transform.localPosition;
        startTime = Time.time;
        journeyLength = Vector3.Distance(transform.position, aimPos.position);
        initialFOV = cam.fieldOfView;
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion gunHolderRot = CalculateSway() * RecoilRotation();
        Vector3 gunHolderPos = Kickback();
        print("PositionBeforeDiff = " + gunHolderPos);
        Quaternion camRot = RecoilRotation();


        if (shoot.playerHoldingGun) // If the player is holding a gun.
        {
            bool aimKeyDown = Input.GetKey(aimKey);
            Vector3 aim = Aim(aimKeyDown); // Work out the position of the gun if we're currently scoping in or out.
            Vector3 positionDifference = targetPosition - aim;
            gunHolderPos -= positionDifference;
            print("PositionAfterDiff = " + gunHolderPos);

            // Need to find a way to implement the below properly.
            //gunHolderPos += Aim(Input.GetKey(aimKey));

            if (Vector3.Distance(transform.localPosition, aimPos.localPosition) < .01f && !playerAiming && aimKeyDown) // Gun holder has reached the aiming position.
            {
                playerAiming = true;
                startTime = Time.time;
                print("playerAiming = " + playerAiming + "(t" + startTime + ")");
            }
            else if (Vector3.Distance(transform.localPosition, initialGunPosition) < .01f && playerAiming && !aimKeyDown) // Gun holder has reached the original pos again.
            {
                playerAiming = false;
                startTime = Time.time;
                print("PlayerNotAiming = " + playerAiming + "(t" + startTime + ")");
            }
        }

        debugPosition = aimPos.position;
        transform.localRotation = gunHolderRot;
        cam.transform.localRotation = camRot;
        transform.localPosition = gunHolderPos;
        // cam.transform.localRotation = Quaternion.Euler(gunHolderRot.eulerAngles);
    }

    Vector3 Aim(bool aim)
    {
        float distCovered = (Time.time - startTime) * aimSpeed; // Distance moved equals elapsed time times speed..
        float fractionOfJourney = distCovered / journeyLength; // Fraction of journey completed equals current distance divided by total distance.
        Vector3 result;

        if (aim)
        {
            result = Vector3.Slerp(initialGunPosition, aimPos.localPosition, fractionOfJourney);
            cam.fieldOfView = Mathf.Lerp(initialFOV, ADSFov, fractionOfJourney);
        }
        else
        {
            result = Vector3.Slerp(aimPos.localPosition, initialGunPosition, fractionOfJourney);
            cam.fieldOfView = Mathf.Lerp(ADSFov, initialFOV, fractionOfJourney);
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
        targetRotation.eulerAngles *= Time.deltaTime * smooth;
        Quaternion swayAngle = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);
        return swayAngle;
    }

    Quaternion RecoilRotation()
    {
        recoilRotation = Vector3.Lerp(recoilRotation, Vector3.zero, Time.deltaTime * returnAmount);
        currentRotation = Vector3.Slerp(currentRotation, recoilRotation, Time.fixedDeltaTime * snappiness);
        return Quaternion.Euler(currentRotation);
    }

    public void Recoil() // Called from shoot.cs when the player shoots.
    {
        Vector3 kickBack = new Vector3(0f, 0f, kickbackZ);
        Vector3 recoil = new Vector3(recoilX, Random.Range(0f, recoilY), Random.Range(-recoilZ, recoilZ));
        kickBack *= -1f;
        recoil *= -1f;

        print("kickBack = " + kickBack + ", recoil = " + recoil);

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