using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;

public class GunRecoil : MonoBehaviour
{
    private Vector3 currentRotation;
    private Vector3 targetRotation;
    private Vector3 targetPosition;
    private Vector3 currentPosition;

    [SerializeField] private KeyCode aimKey = KeyCode.Mouse1;
    [SerializeField] private Camera cam;


    [SerializeField] public float recoilX, recoilY, recoilZ, kickBack;
    [SerializeField] public float snappiness, returnSpeed, notShootingReturnMultiplier, swayMultiplier, swaySmooth, AimSpeed;
    [SerializeField] public Shoot shootScript;

    // Variables used to control aiming.
    [SerializeField] private Transform aimPos;
    private Vector3 initialGunPosition;
    private bool playerTriggeredAim = false, playerAiming = false;
    private float keyDownTime, keyUpTime, startTime, journeyLength;
    private Quaternion swayRotation;


    private void Start()
    {
        initialGunPosition = transform.localPosition;
        journeyLength = Vector3.Distance(transform.position, aimPos.position);
    }

    private void Update()
    {
        float realReturnSpeed = returnSpeed;

        if (!shootScript.firing)
        {
            realReturnSpeed *= notShootingReturnMultiplier;
        }

        // Recoil rotations
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.deltaTime);

        Quaternion recoilRotation = Quaternion.Euler(currentRotation);
        Quaternion swayAngle = CalculateSway();
        Quaternion combinedRotation = recoilRotation * swayAngle;


        // Recoil positions
        Vector3 kickbackPosition = Kickback();
        Vector3 aimingPosition = Aim(AimCheck());
        Vector3 finalPosition = kickbackPosition - (targetPosition - aimingPosition);

        // Applying the rotation and position.
        transform.localRotation = combinedRotation;
        transform.localPosition = finalPosition;

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
        swayRotation = rotationX * rotationY;

        Quaternion swayAngle = Quaternion.Slerp(transform.localRotation, swayRotation, swaySmooth * Time.deltaTime);
        return swayAngle;
    }


    Vector3 Kickback()
    {
        targetPosition = Vector3.Lerp(targetPosition, initialGunPosition, Time.deltaTime * returnSpeed); // Moves from the gun's last target position back to the initial gun position.
        currentPosition = Vector3.Lerp(currentPosition, targetPosition, Time.fixedDeltaTime * snappiness); // Moves the gun from the current position to the target position.
        return currentPosition; // Unless aiming or firing, 'currentPosition' will be equal to the initialGunPosition.
    }


    public void RecoilFire()
    {
        targetRotation += new Vector3(-recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));

        Vector3 kickBackPosition = new Vector3(0f, 0f, kickBack);
        targetPosition -= kickBackPosition;
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
            cam.fieldOfView = Mathf.LerpAngle(90f, 45f, fractionOfJourney);
        }
        else
        {
            result = Vector3.Slerp(aimPos.localPosition, initialGunPosition, fractionOfJourney);
            cam.fieldOfView = Mathf.LerpAngle(45f, 90f, fractionOfJourney);
        }

        return result;
    }
}
