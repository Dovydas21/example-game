using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerInput playerInput;
    private PlayerInput.OnFootActions onFoot;
    private Shoot shoot;
    public GunInfo gunInfo;
    private Vector2 movementInput;
    private PlayerMotor motor;
    private PlayerLook look;
    private bool buttonPressed;

    // Coroutines allow actions to be preformed across several frames, can pause and start execution quickly,
    // important for snappy shoot
    // ing 
    Coroutine fireCoroutine;
    // Start is called before the first frame update
    void Awake()
    {
        playerInput = new PlayerInput();
        onFoot = playerInput.OnFoot;
        motor = GetComponent<PlayerMotor>();
        look = GetComponent<PlayerLook>();
        shoot = GetComponent<Shoot>();
        /** Events
         * Anytime output jump is performed, a callback context (ctx) is used to call the "motor.Jump" function
         * callback context gives context on event, can send context to function (lookup lambda expressions)
         * Jump has 3 states, depending on functionality 
        **/
        onFoot.Jump.performed += ctx => motor.Jump();
        onFoot.Fire.started += _ => StartFiring();
        onFoot.Fire.canceled += _ => StopFiring();
        onFoot.Aim.performed += ctx => shoot.Aim();
        onFoot.Dropobject.performed += ctx => shoot.Drop();
    }

    // Update is called once per frame
    void Update()
    {
        motor.ProcessMove(onFoot.Movement.ReadValue<Vector2>());
    }
    void StartFiring()
    {
        fireCoroutine = StartCoroutine(shoot.FullAuto());
    }

    void StopFiring()
    {
        if (fireCoroutine != null)
        {
            StopCoroutine(fireCoroutine);
        }
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
