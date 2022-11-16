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


    private void Start()
    {
        initialGunPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion gunHolderRot = CalculateSway();
        Vector3 gunHolderPos = CalculateRecoil();

        print("gunHolderRot = " + gunHolderRot);
        print("gunHolderPos = " + gunHolderPos);

        transform.localRotation = gunHolderRot; // Not working at the moment as line 77 is overwriting this rotation...
        transform.localPosition = gunHolderPos;
    }

    Quaternion CalculateSway()
    {
        // Get mouse input.
        float mouseX = Input.GetAxisRaw("Mouse X") * swayMultiplier;
        float mouseY = Input.GetAxisRaw("Mouse Y") * swayMultiplier;
        print("MouseInput, X = " + mouseX + ", Y = " + mouseY);

        // Work out the rotation.
        Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(-mouseX, Vector3.up);
        print("MouseAngles, rotationX = " + rotationX + ", rotationY = " + rotationY);

        // Work out where we are ultimately moving the gun to.
        targetRotation = rotationX * rotationY;
        targetRotation.eulerAngles *= Time.deltaTime * smooth;
        Quaternion swayAngle = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);
        print("Sway angle = " + swayAngle);
        return swayAngle;
    }

    Vector3 CalculateRecoil()
    {
        recoilRotation = Vector3.Lerp(recoilRotation, Vector3.zero, Time.deltaTime * returnAmount);
        currentRotation = Vector3.Slerp(currentRotation, recoilRotation, Time.fixedDeltaTime * snappiness);
        transform.localRotation = Quaternion.Euler(currentRotation);
        Vector3 kickback = Kickback();
        return kickback + currentRotation;
    }

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

    public Vector3 GetSwayAngle()
    {
        return targetRotation.eulerAngles * -1f;
    }
}