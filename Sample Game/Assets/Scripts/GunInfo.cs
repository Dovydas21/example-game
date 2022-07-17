using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GunInfo : MonoBehaviour
{
    public bool fullAuto;
    public float power;
    public float range;
    public float fireRate;
    public float aimedInFOV;
    public Vector3 defaultGunPosition, defaultGunAngles;
    public Vector3 aimedInPosition, aimedInAngle;
    public string animationName;
    Animator gunAnimation;

    public void Start()
    {
        gunAnimation = gameObject.GetComponentInChildren<Animator>();
    }

    public void PlayShootAnimation()
    {
        gunAnimation.SetTrigger("Fire");
    }

}
