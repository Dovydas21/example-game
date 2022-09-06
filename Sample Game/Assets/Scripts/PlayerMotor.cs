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
    
    float horizontalInput;
    float verticalInput;
    public Transform orientation;
    Vector3 moveDirection;
    Vector3 playerGroundSpot;
    


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
        RaycastHit hitInfo;
        IsGrounded = Physics.Raycast(transform.position, Vector3.down, out hitInfo, 2f);
        playerGroundSpot = hitInfo.point;

        Debug.DrawRay(transform.position, Vector3.down, Color.red, .5f);
        print("IsGrounded = " + IsGrounded);
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
        if (IsGrounded) // Ground check
        {
            sfx.PlayGrunt();
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
            // rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawSphere(playerGroundSpot, .3f);
    }
}
 