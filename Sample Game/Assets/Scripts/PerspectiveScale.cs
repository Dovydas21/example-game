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
    private Vector3 newPosition;
    private Vector3 lowestBounds;
    private List<Vector3> collisionPoints = new List<Vector3>();



    private void Start()
    {
        mainCamera = Camera.main;
        collisionLayerMask = LayerMask.GetMask("Ground"); // Adjust the layer name if needed
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
                //ToggleObjectColliders(false); // Disable the colliders.
                
            }
        }
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

    private void ResizeObject()
    {
        float distance = Vector3.Distance(mainCamera.transform.position, objectTransform.position);
        float clampedDistance = Mathf.Clamp(distance, 0f, maxDistance);
        float scaleMultiplier = clampedDistance / initialScale.magnitude;


        Vector3 direction = mainCamera.transform.forward;
        //Vector3 newPosition = mainCamera.transform.position + direction.normalized * (clampedDistance + scaleMultiplier);

        objectTransform.localScale = initialScale * scaleMultiplier;

        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)); // Cast ray from the center of the screen
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxDistance, LayerMask.GetMask("Default")))
        {
            cursorPos = hit.point;
        }
        else
        {
            cursorPos = ray.GetPoint(clampedDistance);
        }

        newPosition = cursorPos;

        //newPosition = GetAdjustedPosition(newPosition);



        objectTransform.position = newPosition;
    }

    private Vector3 GetAdjustedPosition(Vector3 position)
    {
        Vector3 currentPosition = objectTransform.position;

        Vector3 lowestVertexPosition = FindLowestVertex(objectTransform);

        if (currentPosition.y < lowestVertexPosition.y + floorHeight.transform.position.y)
        {
            float offset = floorHeight.transform.position.y - (lowestVertexPosition.y + floorHeight.transform.position.y);
            currentPosition.y += offset;
            objectTransform.position = currentPosition;
        }

        return position;

        //float positionDiff = newPosition.y;
        //if (FindLowestVertex(objectTransform).y <= floorHeight.transform.position.y)
        //{
        //    positionDiff = Mathf.Abs(newPosition.y - FindLowestVertex(objectTransform).y);
        //}

        //return new Vector3(position.x, positionDiff, position.z);
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


    //private void OnCollisionEnter(Collision other)
    //{
    //    if(other.transform.tag != "Player")
    //    {
    //        Vector3 direction = objectTransform.position - other.transform.position;
    //        float distance = direction.magnitude;
    //        Vector3 collisionPoint = other.collider.ClosestPoint(other.transform.position);
    //        collisionPoints.Add(collisionPoint);
    //        if (distance < 3f)
    //        {
    //            newPosition -= direction.normalized;
    //        }
    //    }
    //}


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawSphere(cursorPos, .3f);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(newPosition, .1f);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(FindLowestVertex(objectTransform), .1f);

        Gizmos.color = Color.green;
        float positionDiff = newPosition.y;
        if (FindLowestVertex(objectTransform).y <= floorHeight.transform.position.y)
        {
            positionDiff = Mathf.Abs(newPosition.y - FindLowestVertex(objectTransform).y);
        }

        foreach(Vector3 point in collisionPoints)
        {
            Gizmos.DrawCube(point, new Vector3(.1f, .1f, .1f));
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
}



//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class PerspectiveScale : MonoBehaviour
//{
//    private Camera mainCamera;
//    private bool isPickedUp = false;
//    private Transform objectTransform;
//    private Vector3 initialScale;
//    private Vector3 initialPosition;
//    private float maxDistance = 10f;
//    private float collisionCheckRadius = 1f;
//    private LayerMask collisionLayerMask;
//    private float groundOffset = 0f;

//    private void Start()
//    {
//        mainCamera = Camera.main;
//        collisionLayerMask = LayerMask.GetMask("Ground"); // Adjust the layer name if needed
//    }

//    private void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.T))
//        {
//            if (isPickedUp)
//            {
//                DropObject();
//            }
//            else
//            {
//                PickUpObject();
//            }
//        }

//        if (isPickedUp)
//        {
//            ResizeObject();
//        }
//    }

//    private void PickUpObject()
//    {
//        RaycastHit hit;
//        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit))
//        {
//            float distance = Vector3.Distance(mainCamera.transform.position, hit.point);
//            if (distance <= maxDistance)
//            {
//                isPickedUp = true;
//                objectTransform = hit.transform;
//                initialScale = objectTransform.localScale;
//                initialPosition = objectTransform.position;
//            }
//        }
//    }

//    private void ResizeObject()
//    {
//        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)); // Cast ray from the center of the screen
//        RaycastHit hit;
//        if (Physics.Raycast(ray, out hit))
//        {
//            float distance = Vector3.Distance(mainCamera.transform.position, hit.point);
//            float clampedDistance = Mathf.Clamp(distance, 0f, maxDistance);
//            float scaleMultiplier = clampedDistance / initialScale.magnitude;
//            objectTransform.localScale = initialScale * scaleMultiplier;

//            Vector3 direction = hit.point - mainCamera.transform.position;
//            Vector3 newPosition = mainCamera.transform.position + direction.normalized * clampedDistance;
//            newPosition = GetAdjustedPosition(newPosition);
//            objectTransform.position = newPosition;
//        }
//    }

//    private Vector3 GetAdjustedPosition(Vector3 position)
//    {
//        RaycastHit hit;
//        if (Physics.Raycast(position, Vector3.down, out hit, Mathf.Infinity, collisionLayerMask))
//        {
//            position.y = hit.point.y + groundOffset;
//        }
//        return position;
//    }

//    private void DropObject()
//    {
//        isPickedUp = false;
//        objectTransform = null;
//    }
//}
