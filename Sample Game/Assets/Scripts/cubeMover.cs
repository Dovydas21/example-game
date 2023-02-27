using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cubeMover : MonoBehaviour
{
    Vector3 startPos, targetPos; // Position we started at and the position we're trying to get to.
    float startTime; // The time we started moving the object.
    float distanceBetweenPoints; // The distance between the start position and the target position.
    public float speed = 30f; // The speed at which we move the object to the target position.

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        targetPos = new Vector3(100f, 100f, 100f);
        startTime = Time.time;
        distanceBetweenPoints = Vector3.Distance(startPos, targetPos);
    }

    // Update is called once per frame
    void Update()
    {
        float distanceCovered = (Time.time - startTime) * speed; // Work out the speed...
        float fractionOfJourney = distanceCovered / distanceBetweenPoints; // Work out the distance to move the object this frame...
        transform.position = Vector3.Lerp(startPos, targetPos, fractionOfJourney); // Move the object.
    }
}
