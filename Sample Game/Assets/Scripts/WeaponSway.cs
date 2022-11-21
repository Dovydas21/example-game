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
    public Transform cam;
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
    float startTime;
    float journeyLength;

    Vector3 debugPosition;
    Vector3 lastDebugPosition;

    private void Start()
    {
        initialGunPosition = transform.localPosition;
        startTime = Time.time;
        journeyLength = Vector3.Distance(transform.position, aimPos.position);
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion gunHolderRot = CalculateSway() * RecoilRotation();
        Vector3 gunHolderPos = Kickback();
        if (shoot.playerHoldingGun)
            gunHolderPos += Aim(Input.GetKey(aimKey));
            //gunHolderPos = Aim(Input.GetKey(aimKey)); // Works here if we don't add the aiming value to the kickback value...

        debugPosition = aimPos.localPosition;
        transform.localRotation = gunHolderRot;
        transform.localPosition = gunHolderPos;
        cam.localRotation = Quaternion.Euler(gunHolderRot.eulerAngles * -1f);
        //cam.position = gunHolderPos;
    }

    Vector3 Aim(bool aim)
    {
        float distCovered = (Time.time - startTime) * aimSpeed; // Distance moved equals elapsed time times speed..
        float fractionOfJourney = distCovered / journeyLength; // Fraction of journey completed equals current distance divided by total distance.
        Vector3 result;

        if (!aim)
        {
            print("Player STOPPED aiming.");
            result = Vector3.Slerp(initialGunPosition, aimPos.localPosition, fractionOfJourney);
        }
        else
        {
            print("Player STARTED aiming.");
            result = Vector3.Slerp(aimPos.localPosition, initialGunPosition, fractionOfJourney);
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
        print("Sway angle = " + swayAngle);
        return swayAngle;
    }

    Quaternion RecoilRotation()
    {
        recoilRotation = Vector3.Lerp(recoilRotation, Vector3.zero, Time.deltaTime * returnAmount);
        currentRotation = Vector3.Slerp(currentRotation, recoilRotation, Time.fixedDeltaTime * snappiness);
        return Quaternion.Euler(currentRotation);
    }

    /*Vector3 CalculateRecoil()
    {
        recoilRotation = Vector3.Lerp(recoilRotation, Vector3.zero, Time.deltaTime * returnAmount);
        currentRotation = Vector3.Slerp(currentRotation, recoilRotation, Time.fixedDeltaTime * snappiness);
        transform.localRotation = Quaternion.Euler(currentRotation);
        Vector3 kickback = Kickback();
        return kickback + currentRotation;
    }*/

    public void Recoil()
    {
        // Global here for the recoil pos? Then we can just add the current pos to the recoil pos in Update(){} and then CalculateRecoil() only needs to work out angles.
        targetPosition -= new Vector3(0f, 0f, kickbackZ);
        targetPosition += new Vector3(recoilX, Random.Range(0f, recoilY), Random.Range(-recoilZ, recoilZ));
    }

    Vector3 Kickback()
    {
        targetPosition = Vector3.Lerp(targetPosition, initialGunPosition, Time.deltaTime * returnAmount);
        currentPosition = Vector3.Lerp(currentPosition, targetPosition, Time.fixedDeltaTime * snappiness);
        //transform.localPosition = currentPosition;
        return currentPosition;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        if(lastDebugPosition != debugPosition)
        {
            lastDebugPosition = debugPosition;
            print("GunHolder moved " + Vector3.Distance(lastDebugPosition, debugPosition));
        }
        Gizmos.DrawSphere(debugPosition, .1f);
    }
}