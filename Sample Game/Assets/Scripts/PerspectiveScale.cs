using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PerspectiveScale : MonoBehaviour
{
    private Camera mainCamera;
    private bool isPickedUp = false;
    private Transform objectTransform;
    private Vector3 initialScale;
    private Vector3 initialPosition;
    private float initialDistance;
    private float maxDistance = 15f;
    private LayerMask collisionLayerMask;
    private float groundOffset = 0f;
    [SerializeField]
    private GameObject floorHeight;
    private Vector3 cursorPos;
    private Vector3 highestVertPos;
    private Vector3 newPosition;
    private Vector3 lowestBounds;
    private List<Vector3> collisionPoints = new List<Vector3>();



    private void Start()
    {
        mainCamera = Camera.main;
        collisionLayerMask = LayerMask.GetMask("Default"); // Adjust the layer name if needed
    }

    private void Update()
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
            ResizeObject();
            AdjustPos();
        }
    }

    private void PickUpObject()
    {
        RaycastHit hit;
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit))
        {
            float distance = Vector3.Distance(mainCamera.transform.position, hit.point);
            if (distance <= maxDistance)
            {
                isPickedUp = true;
                objectTransform = hit.transform;
                initialScale = objectTransform.localScale;
                initialPosition = objectTransform.position;
                initialDistance = distance;
                ChangeCollisionLayer(LayerMask.GetMask("Ignore Raycast"));
                objectTransform.GetComponent<Rigidbody>().isKinematic = true;
                ToggleObjectColliders(false); // Disable the colliders.
            }
        }
    }


    private void DropObject()
    {
        ToggleObjectColliders(true); // Enable the colliders.
        isPickedUp = false;
        objectTransform.GetComponent<Rigidbody>().isKinematic = false;
        ChangeCollisionLayer(LayerMask.GetMask("Default"));
        objectTransform = null;
    }


    private void ResizeObject()
    {
        float distance = Vector3.Distance(mainCamera.transform.position, objectTransform.position);
        float clampedDistance = Mathf.Clamp(distance, 1f, maxDistance);
        float scaleMultiplier = clampedDistance / initialScale.magnitude;


        Vector3 direction = mainCamera.transform.forward;
        //Vector3 newPosition = mainCamera.transform.position + direction.normalized * (clampedDistance + scaleMultiplier);

        objectTransform.localScale = initialScale * scaleMultiplier;

        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)); // Cast ray from the center of the screen

        cursorPos = ray.GetPoint(clampedDistance); 
        newPosition = cursorPos; // Set newPosition to the location that the player's cursor is at.

        objectTransform.position = newPosition; // Set the position of the picked up object.
    }

    private void AdjustPos() // Adjusts the position of the picked up object so that it is always above the floor plane.
    {
        Vector3 lowestVertPos = FindLowestVertex(objectTransform);

        if (lowestVertPos.y > floorHeight.transform.position.y)
        {
            return; // If the lowest Y position (up & down) of the object is above the floor plane then exit this function.
        }
        else
        {
            RaycastHit hitInfo;
            bool rayHit = Physics.Raycast(lowestVertPos, Vector3.up, out hitInfo, Mathf.Infinity);
            if (rayHit)
            {
                Vector3 highestVertPos = FindHighestVertex(hitInfo.transform);
                //float posDiff = Mathf.Abs(hitInfo.point.y - highestVertPos.y);
                print("hitInfo.magnitude = " + hitInfo.transform.localScale.magnitude);
                float posDiff = Vector3.Distance(hitInfo.point, lowestVertPos);
                objectTransform.position += new Vector3(0f, posDiff, 0f);
                print(posDiff);
            }
        }
    }

    private Vector3 FindHighestVertex(Transform targetTransform)
    {
        Vector3 highestVertex;
        try
        {
            Mesh mesh = targetTransform.GetComponent<MeshFilter>().sharedMesh;
            Vector3[] vertices = mesh.vertices;
            highestVertex = targetTransform.TransformPoint(vertices[0]);

            for (int i = 1; i < vertices.Length; i++)
            {
                Vector3 worldPosition = targetTransform.TransformPoint(vertices[i]);
                if (worldPosition.y > highestVertex.y)
                {
                    highestVertex = worldPosition;
                }
            }
        }
        catch
        {
            highestVertex = default;
        }

        return highestVertex;
    }

    private Vector3 FindLowestVertex(Transform targetTransform)
    {
        Vector3 lowestVertex;
        try
        {
            Mesh mesh = targetTransform.GetComponent<MeshFilter>().sharedMesh;
            Vector3[] vertices = mesh.vertices;
            lowestVertex = targetTransform.TransformPoint(vertices[0]);

            for (int i = 1; i < vertices.Length; i++)
            {
                Vector3 worldPosition = targetTransform.TransformPoint(vertices[i]);
                if (worldPosition.y < lowestVertex.y)
                {
                    lowestVertex = worldPosition;
                }
            }
        }
        catch
        {
            lowestVertex = default;
        }

        return lowestVertex;
    }

    void ToggleObjectColliders(bool state)
    {
        Collider[] parentColliders = objectTransform.GetComponents<Collider>();

        foreach (Collider col in parentColliders)
        {
            col.enabled = state;
        }

        Collider[] childColliders = objectTransform.GetComponentsInChildren<Collider>(true);

        foreach (Collider col in childColliders)
        {
            col.enabled = state;
        }
    }

    void ChangeCollisionLayer(LayerMask layer)
    {

        objectTransform.gameObject.layer = layer;

        try
        {

            GameObject[] children = objectTransform.GetComponentsInChildren<GameObject>(true);

            if (children != null)
            {
                foreach (GameObject child in children)
                {
                    child.layer = layer;
                }
            }
        }
        catch { }

    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawSphere(cursorPos, .3f);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(newPosition, .1f);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(FindLowestVertex(objectTransform), .1f);
        Gizmos.DrawSphere(highestVertPos, .1f);
            
        Gizmos.color = Color.green;
        float positionDiff = newPosition.y;
        if (FindLowestVertex(objectTransform).y <= floorHeight.transform.position.y)
        {
            positionDiff = Mathf.Abs(newPosition.y - FindLowestVertex(objectTransform).y);
        }

        foreach (Vector3 point in collisionPoints)
        {
            Gizmos.DrawCube(point, new Vector3(.1f, .1f, .1f));
        }
    }

}