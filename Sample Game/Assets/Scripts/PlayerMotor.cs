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
        // controller = GetComponent<CharacterController>();
        sfx = GetComponent<SFXScript>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        MyInput();

        if (Input.GetKeyDown(jumpKey))
        {
            print("Jump key pressed");
            Jump();
        }

        if (Input.GetKeyDown(dropWeaponKey))
        {
            GunInfo gunInfo = GetComponentInChildren<GunInfo>();
            if (gunInfo != null)
            {
                StartCoroutine(gunInfo.Drop());
            }
        }
    }

    void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        rb.AddForce(moveDirection.normalized * speed * 10f, ForceMode.VelocityChange);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        GroundCheck();
        MovePlayer();
        LimitSpeed();
    }

    void LimitSpeed()
    {
        // Speed limitation.
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if (flatVel.magnitude > speed)
        {
            Vector3 limitedVel = flatVel.normalized * speed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    void GroundCheck()
    {
        IsGrounded = Physics.Raycast(transform.position, Vector3.down, 2f);
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
        rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange); // Apply the jump force to the player.
    }
}
