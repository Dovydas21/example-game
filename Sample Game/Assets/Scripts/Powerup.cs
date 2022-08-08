using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{

    public Collider pickupTrigger;
    public enum PowerupTypes { SpeedUpgrade, JumpUpgrade };
    public PowerupTypes type;


    private void OnTriggerEnter(Collider pickupTrigger)
    {
        if (pickupTrigger.tag == "Player")
        {
            Destroy(gameObject);

            GameObject player = pickupTrigger.gameObject;
            PlayerMotor motor = player.GetComponent<PlayerMotor>();

            if (type == PowerupTypes.SpeedUpgrade)
            {
                motor.speed += 10f;
                motor.runSpeed += 10f;
            }
            else if (type == PowerupTypes.JumpUpgrade)
            {
                motor.jumpHeight += 10f;
            }
        }
    }
}
