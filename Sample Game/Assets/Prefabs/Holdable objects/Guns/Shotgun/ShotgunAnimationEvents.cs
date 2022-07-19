using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunAnimationEvents : MonoBehaviour
{
    void ResetTrigger()
    {
        gameObject.GetComponent<Animator>().ResetTrigger("Fire");
        print("Shotgun animation reset");
    }
}
