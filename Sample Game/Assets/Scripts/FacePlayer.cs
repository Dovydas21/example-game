using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.UIElements;

public class FacePlayer : MonoBehaviour
{
    public GameObject playerObj;

    void FixedUpdate()
    {
        Vector3 direction = (playerObj.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
        transform.rotation = lookRotation;
    }
}
