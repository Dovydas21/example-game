using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRecoil : MonoBehaviour
{
    private Vector3 currentRotation;
    private Vector3 targetRotation;


    [SerializeField] private float recoilX, recoilY, recoilZ;
    [SerializeField] private float snappiness, returnSpeed;


    private void Update()
    {
        //if (Input.GetKey(KeyCode.Mouse0)) RecoilFire();

        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.deltaTime);

        transform.localRotation = Quaternion.Euler(currentRotation);
    }

    public void RecoilFire()
    {
        targetRotation += new Vector3(-recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));

    }
}
