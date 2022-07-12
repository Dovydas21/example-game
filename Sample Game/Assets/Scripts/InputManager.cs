using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerInput playerInput;
    private PlayerInput.OnFootActions onFoot;
    private Vector2 movementInput;
    private PlayerMotor motor;
    private PlayerLook look;
    // Start is called before the first frame update
    void Awake()
    {   
        Cursor.visible = false;
        playerInput = new PlayerInput();
        onFoot = playerInput.OnFoot;
        motor = GetComponent<PlayerMotor>();
        /** Events
         * Anytime output jump is performed, a callback context (ctx) is used to call the "motor.Jump" function
         * Jump has 3 states, depending on functionality 
        **/
        onFoot.Jump.performed += ctx => motor.Jump();
        
        look = GetComponent<PlayerLook>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        motor.ProcessMove(onFoot.Movement.ReadValue<Vector2>());
    }

    private void LateUpdate()
    {
        look.ProcessLook(onFoot.Look.ReadValue<Vector2>());
    }

    private void OnEnable()
    {
        onFoot.Enable();
    }
    private void OnDisable()
    {
        onFoot.Disable();
    }
}
