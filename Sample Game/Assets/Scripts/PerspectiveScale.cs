using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerspectiveScale : MonoBehaviour
{
    private Camera mainCamera;
    private bool isPickedUp = false;
    private Vector3 objectOffset;
    private Transform objectTransform;
    private float objectDistance;
    private float objectScale;
    private Vector3 rayPoint;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (isPickedUp)
            {
                DropObject();
            }
            else
            {
                PickUpObject();
            }
        }
        if (isPickedUp)
        {
            MoveObject();
        }
    }

    void PickUpObject()
    {
        RaycastHit hit;
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit))
        {
            isPickedUp = true;
            objectTransform = hit.transform;
            objectOffset = transform.position - hit.point;
            objectDistance = hit.distance;
            objectScale = transform.localScale.x;
        }
    }

    void MoveObject()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        /*Vector3 rayDirection = ray.direction.normalized;
        Vector3 objectDirection = (objectTransform.position - mainCamera.transform.position).normalized;
        float dotProduct = Vector3.Dot(rayDirection, objectDirection);
        float scaleMultiplier = Mathf.Max(0.5f, dotProduct);*/
        float scaleMultiplier = objectDistance;
        objectTransform.localScale = new Vector3(objectScale * scaleMultiplier, objectScale * scaleMultiplier, objectScale * scaleMultiplier);
        Vector3 rayPoint = ray.GetPoint(objectDistance);
        objectTransform.position = rayPoint;
    }

    /*
     * void MoveObject()
    {
        ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Vector3 rayDirection = ray.direction.normalized;
        Vector3 objectDirection = (objectTransform.position - mainCamera.transform.position).normalized;
        float dotProduct = Vector3.Dot(rayDirection, objectDirection);
        float scaleMultiplier = Mathf.Max(0.5f, dotProduct);
        objectTransform.localScale = new Vector3(objectScale * scaleMultiplier, objectScale * scaleMultiplier, objectScale * scaleMultiplier);
        Vector3 rayPoint = ray.GetPoint(objectDistance / dotProduct);
        objectTransform.position = rayPoint + objectOffset;
    }
     * 
     * void MoveObject()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Vector3 rayPoint = ray.GetPoint(objectDistance);
        objectTransform.position = rayPoint;
        float scaleMultiplier = mainCamera.transform.position.z / objectDistance;
        objectTransform.localScale = new Vector3(objectScale * scaleMultiplier, objectScale * scaleMultiplier, objectScale * scaleMultiplier);
    }*/

    private void OnDrawGizmos()
    {
       Gizmos.DrawCube(rayPoint, new Vector3(.1f, .1f, .1f));
    }

    void DropObject()
    {
        objectTransform = null;
        isPickedUp = false;
    }
}