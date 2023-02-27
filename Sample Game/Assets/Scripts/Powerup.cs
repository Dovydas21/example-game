using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{

    public Collider pickupTrigger;
    public enum PowerupTypes { SpeedUpgrade, JumpUpgrade };
    public PowerupTypes type;
    public float duration;
    bool pickedUp;
    PlayerMotor motor;


    private void Start()
    {
        pickedUp = false;
    }

    private void OnTriggerEnter(Collider pickupTrigger)
    {
        if (pickupTrigger.tag == "Player" && !pickedUp)
        {
            pickedUp = true;
            GameObject player = pickupTrigger.gameObject;
            motor = player.GetComponent<PlayerMotor>();

            if(motor != null) StartCoroutine(ApplyPowerup());
        }
    }

    IEnumerator ApplyPowerup()
    {
        gameObject.transform.localScale = new Vector3(0f, 0f, 0f); // Set current object's scale to (0, 0, 0) to hide it. We cannot destroy it until the coroutine is finished.

        if (type == PowerupTypes.SpeedUpgrade)
        {
            motor.baseSpeed += 10f;
            motor.speed += 10f;
            motor.runSpeed += 10f;

            print("Speed powerup active!");
            
            yield return new WaitForSeconds(duration);
            print("Speed powerup timed out!");

            motor.baseSpeed -= 10f;
            motor.speed -= 10f;
            motor.runSpeed -= 10f;

        }
        else if (type == PowerupTypes.JumpUpgrade)
        {
            motor.jumpHeight += 10f;
        }

        Destroy(gameObject); // Destroy the gameobject with this script attached to, we do not need it after it has been applied.
    }
}
