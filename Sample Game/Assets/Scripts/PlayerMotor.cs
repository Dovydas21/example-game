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
    public float gravity = -9.8f;
    public float jumpHeight = 1f;
    public float baseSpeed;
    private SFXScript sfx;
    Rigidbody rb;

    [Header("Key bindings")]
    public KeyCode moveForwardKey;
    public KeyCode moveBackwardKey;
    public KeyCode moveLeftKey;
    public KeyCode moveRightKey;



    // Start is called before the first frame update
    void Start()
    {
        // controller = GetComponent<CharacterController>();
        sfx = GetComponent<SFXScript>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        GroundCheck();

        if (Input.GetKey(moveForwardKey))
            rb.AddForce(Camera.main.transform.forward * speed, ForceMode.Acceleration);

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

    public void ProcessMove(Vector2 input)
    {
        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;
        // controller.Move(transform.TransformDirection(moveDirection) * speed * Time.deltaTime);
        playerVelcocity.y += gravity * Time.deltaTime;
        if (IsGrounded && playerVelcocity.y < 0)
            playerVelcocity.y = -2f;
        // controller.Move(playerVelcocity * Time.deltaTime);
    }

    public void Jump()
    {
        if (IsGrounded)
        {
            sfx.PlayGrunt();
            Debug.Log("Jumped");
            playerVelcocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravity);
        }
    }
}
