using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    // private CharacterController controller;
    private Vector3 playerVelcocity;
    private bool IsGrounded;
    public float speed = 5f;
    public float runSpeed = 10f;
    public float jumpForce = 100f;
    public float gravity = -9.8f;
    public float jumpHeight = 1f;
    public float baseSpeed;
    private SFXScript sfx;
    Rigidbody rb;

    // Variables that control the spring force
    public float rideHeight = .5f;
    public float springStrength = .5f;
    public float springDamper = .5f;    

    [Header("Key bindings")]
    public KeyCode jumpKey;
    public KeyCode dropWeaponKey;

    float horizontalInput;
    float verticalInput;
    public Transform orientation;
    Vector3 moveDirection;



    // Start is called before the first frame update
    void Start()
    {
        sfx = GetComponent<SFXScript>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        MyInput();
        Fall();
        GroundCheck();
        MovePlayer();
        LimitSpeed();


        if (Input.GetKeyDown(jumpKey))
            Jump();

        if (Input.GetKeyDown(dropWeaponKey))
        {
            GunInfo gunInfo = GetComponentInChildren<GunInfo>();
            if (gunInfo != null)
                StartCoroutine(gunInfo.Drop());
        }
    }

    void MyInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");



        /* if ( horizontalInput + verticalInput == 0 && IsGrounded)
        {
            print("horizontalInput = " + horizontalInput);
            print("verticalInput = " + verticalInput);
            rb.isKinematic = true;
        }
        rb.isKinematic = false;
        */
    }

    void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        rb.AddForce(moveDirection.normalized * speed * 10f, ForceMode.Acceleration);
        // rb.AddForce(moveDirection.normalized * speed * 10f, ForceMode.Force);
    }

    void FloatPlayer()
    {
        RaycastHit hitInfo;
        bool rayHit = Physics.Raycast(transform.position, Vector3.down, out hitInfo);
        if (rayHit)
        {
            Vector3 velocity = rb.velocity;
            Vector3 rayDirection = transform.TransformDirection(Vector3.down);
            Vector3 otherVelocity = Vector3.zero;
            Rigidbody hitBody = hitInfo.rigidbody;

            if (hitBody != null)
                otherVelocity = hitBody.velocity;

            float rayDirectionVelocity = Vector3.Dot(rayDirection, velocity);
            float otherDirectionVelocity = Vector3.Dot(rayDirection, otherVelocity);

            float relativeVelocity = rayDirectionVelocity - otherDirectionVelocity;

            float x = hitInfo.distance - rideHeight;
            float springForce = (x * springStrength) - (relativeVelocity * springDamper);
            rb.AddForce(rayDirection * springForce);
            Debug.DrawLine(transform.position, transform.position + (rayDirection * springForce), Color.magenta);
        }
    }

    void LimitSpeed()
    {
        // Speed limitation.
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if (flatVel.magnitude > speed)
        {
            Vector3 limitedVel = flatVel.normalized * speed * Time.deltaTime;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    void GroundCheck()
    {
        IsGrounded = Physics.Raycast(transform.position, Vector3.down, 3f);
    }

    public void StartRunning()
    {
        print("Player has started running...");
        baseSpeed = speed;
        speed = runSpeed;
    }

    public void StopRunning()
    {
        print("Player has stopped running...");
        speed = baseSpeed;
    }

    public void Jump()
    {
        if (!IsGrounded) return; // Exit if the player is not grounded.
        sfx.PlayGrunt(); // Play the jump sound effect.
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // Apply the jump force to the player.
    }

    public void Fall()
    {
        // If the player is falling down from a jump, make them fall faster to make the jump feel betetr.
        if (rb.velocity.y < 0f)
            rb.velocity += Vector3.up * Physics.gravity.y * 1.5f * Time.deltaTime;
    }
}
