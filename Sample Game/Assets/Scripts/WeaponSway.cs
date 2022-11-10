using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    // Variables
    public float swayMultiplier;
    public float smooth;

    Quaternion targetRotation;

    // Update is called once per frame
    void Update()
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
        // transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);
    }

    public Vector3 GetSwayAngle()
    {
        return targetRotation.eulerAngles;
    }
}