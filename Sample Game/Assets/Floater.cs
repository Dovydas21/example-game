using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floater : MonoBehaviour
{
    public Rigidbody rb;
    public float floatingRange = 3f;
    public float floatHeight = 3f;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        rb.AddForce(new Vector3(0f, Mathf.Sin(Time.time) - .5f, 0f), ForceMode.Force);
    }
}
