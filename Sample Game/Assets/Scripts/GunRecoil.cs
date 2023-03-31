using UnityEngine;

public class GunRecoil : MonoBehaviour
{
    // Public variables for customizing recoil parameters
    public float maxRecoilDistance = 0.1f;
    public float recoverySpeed = 2f;
    public float shakeIntensity = 0.1f;
    public float shakeSpeed = 20f;
    public float maxShakeAngle = 10f;

    // Internal variables for tracking recoil state
    private Vector3 recoilStartPosition;
    private float recoilDistance;
    private bool isRecoiling;

    // Internal variables for tracking shake state
    private float shakeIntensityCurrent;
    private float shakeAngleCurrent;

    void Start()
    {
        // Store the initial position of the camera
        recoilStartPosition = transform.localPosition;
    }

    void Update()
    {
        // Check for input to trigger recoil
        if (Input.GetButtonDown("Fire1"))
        {
            // Start the recoil animation
            isRecoiling = true;

            // Start the shake animation
            shakeIntensityCurrent = shakeIntensity;
            shakeAngleCurrent = maxShakeAngle;
        }

        // If we're currently recoiling, move the camera and update the recoil distance
        if (isRecoiling)
        {
            recoilDistance += Time.deltaTime * recoverySpeed;
            float recoilMovement = Mathf.Sin(recoilDistance * Mathf.PI) * maxRecoilDistance * shakeIntensityCurrent;

            // Move the camera in the opposite direction of the recoil movement
            transform.localPosition = recoilStartPosition - (-transform.forward * recoilMovement);

            // If we've reached the maximum recoil distance, stop recoiling
            if (recoilDistance >= 1f)
            {
                isRecoiling = false;
                recoilDistance = 0f;

                // Reset the camera position
                transform.localPosition = recoilStartPosition;
            }
        }

        // If we're currently shaking, apply the shake to the gun rotation
        if (shakeAngleCurrent > 0f)
        {
            float shakeAngle = Mathf.Sin(Time.time * shakeSpeed) * shakeIntensityCurrent * shakeAngleCurrent;
            transform.localRotation = Quaternion.Euler(shakeAngle, 0f, 0f);

            // Reduce the shake intensity and angle over time
            shakeIntensityCurrent -= Time.deltaTime * shakeIntensity;
            shakeAngleCurrent -= Time.deltaTime * maxShakeAngle;
        }
        else
        {
            // Reset the gun rotation
            transform.localRotation = Quaternion.identity;
        }
    }
}
