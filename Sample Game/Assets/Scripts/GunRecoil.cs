using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunRecoil : MonoBehaviour
{ /*
    Vector3 currentRotation;
    Vector3 targetRotation;
    Vector3 targetPosition;
    Vector3 currentPosition;
    Vector3 initialGunPosition;
    Vector3 swayAngle;

    public Transform cam;
    public WeaponSway ws;
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

    // Start is called before the first frame update
    void Start()
    {
        initialGunPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        // RECOIL
        // swayAngle = ws.GetSwayAngle();
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, Time.deltaTime * returnAmount);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, Time.fixedDeltaTime * snappiness);
        transform.localRotation = Quaternion.Euler(currentRotation);
        cam.localRotation = Quaternion.Euler(currentRotation);
        Kickback();
    }

    public void Recoil()
    {
        targetPosition -= new Vector3(0f, 0f, kickbackZ);
        targetPosition += new Vector3(recoilX, Random.Range(0f, recoilY), Random.Range(-recoilZ, recoilZ));
    }

    void Kickback()
    {
        targetPosition = Vector3.Lerp(targetPosition, initialGunPosition, Time.deltaTime * returnAmount);
        currentPosition = Vector3.Lerp(currentPosition, targetPosition, Time.fixedDeltaTime * snappiness);
        transform.localPosition = currentPosition;
    }

    public Vector3 GetRecoilAngle()
    {
        return currentRotation;
    }*/
}
