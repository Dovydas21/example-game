using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    // Start is called before the first frame update
    public Camera cam;
    public GunRecoil recoil;
    public Transform cameraHolder;
    private float xRotation = 0f;
    private float xSensitivity = 100f;
    private float ySensitivity = 100f;

    public void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ProcessLook(Vector2 input)
    {
        float mouseX = input.x;
        float mouseY = input.y;
        // calculate mouse rotation for looking up and down
        xRotation -=(mouseY * Time.deltaTime) * ySensitivity;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        // apply this to camera transform
        Vector3 targetCameraRotation = new Vector3(xRotation, 0f, 0f);
        cameraHolder.localRotation = Quaternion.Euler(targetCameraRotation);
        transform.Rotate(Vector3.up * (mouseX * Time.deltaTime) * xSensitivity);
    }
}