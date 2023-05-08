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


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private float groundOffset = .5f;
    private float floorHeight = 1f;
    private Vector3 cursorPos;



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
                GetComponent<Rigidbody>().isKinematic = true;
                objectTransform.gameObject.layer = 2;
            }
        }
    }

    private void ResizeObject()
    {
        float distance = Vector3.Distance(mainCamera.transform.position, objectTransform.position);
        float clampedDistance = Mathf.Clamp(distance, 0f, maxDistance);
        float scaleMultiplier = clampedDistance / initialScale.magnitude;
        objectTransform.localScale = initialScale * scaleMultiplier;


        Vector3 direction = mainCamera.transform.forward;
        //Vector3 newPosition = mainCamera.transform.position + direction.normalized * (clampedDistance + scaleMultiplier);


        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)); // Cast ray from the center of the screen
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxDistance, 9))
        {
            cursorPos = hit.point;
        }
        else
        {
            cursorPos = ray.GetPoint(clampedDistance);
        }

        Vector3 newPosition = cursorPos;// * (clampedDistance + scaleMultiplier);

        newPosition = GetAdjustedPosition(newPosition);
        objectTransform.position = newPosition;
    }

    private Vector3 GetAdjustedPosition(Vector3 position)
    {
        groundOffset = transform.localScale.x / 2f;
        if (position.y <= floorHeight)
        {
            position.y = floorHeight + groundOffset;
        }
        return position;
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other != objectTransform.GetComponent<Collider>())
    //    {
    //        Vector3 direction = objectTransform.position - other.transform.position;
    //        float distance = direction.magnitude;
    //        Vector3 collisionPoint = other.ClosestPoint(other.transform.position);
    //        if (distance < 3f)
    //        {
    //            other.transform.position = collisionPoint + direction.normalized * 3f;
    //        }
    //    }
    //}

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawSphere(cursorPos, .3f);
    }

    private void DropObject()
    {
        isPickedUp = false;
        objectTransform.gameObject.layer = 0;
        objectTransform = null;
        GetComponent<Rigidbody>().isKinematic = false;
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
//    private float collisionCheckRadius = 2f;
//    private LayerMask collisionLayerMask;
//    private float groundOffset = 0.1f;

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
//        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
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



////using System.Collections;
////using System.Collections.Generic;
////using UnityEngine;

////public class PerspectiveScale : MonoBehaviour
////{
////    private Camera mainCamera;
////    private bool isPickedUp = false;
////    private Transform objectTransform;
////    private Vector3 initialScale;
////    private Vector3 initialPosition;
////    private float maxDistance = 5f;
////    private float collisionCheckRadius = 1f;
////    private LayerMask collisionLayerMask;

////    private void Start()
////    {
////        mainCamera = Camera.main;
////        collisionLayerMask = LayerMask.GetMask("Default"); // Adjust the layer name if needed
////    }

////    private void Update()
////    {
////        if (Input.GetKeyDown(KeyCode.T))
////        {
////            if (isPickedUp)
////            {
////                DropObject();
////            }
////            else
////            {
////                PickUpObject();
////            }
////        }

////        if (isPickedUp)
////        {
////            ResizeObject();
////        }
////    }

////    private void PickUpObject()
////    {
////        RaycastHit hit;
////        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit))
////        {
////            float distance = Vector3.Distance(mainCamera.transform.position, hit.point);
////            if (distance <= maxDistance)
////            {
////                isPickedUp = true;
////                objectTransform = hit.transform;
////                initialScale = objectTransform.localScale;
////                initialPosition = objectTransform.position;
////            }
////        }
////    }

////    private void ResizeObject()
////    {
////        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
////        RaycastHit hit;
////        if (Physics.Raycast(ray, out hit))
////        {
////            float distance = Vector3.Distance(mainCamera.transform.position, hit.point);
////            float clampedDistance = Mathf.Clamp(distance, 0f, maxDistance);
////            float scaleMultiplier = clampedDistance / initialScale.magnitude;
////            objectTransform.localScale = initialScale * scaleMultiplier;

////            Vector3 direction = hit.point - mainCamera.transform.position;
////            Vector3 newPosition = initialPosition + direction.normalized * clampedDistance;
////            CheckCollision(ref newPosition);
////            objectTransform.position = newPosition;
////        }
////    }

////    private void CheckCollision(ref Vector3 position)
////    {
////        Collider[] colliders = Physics.OverlapSphere(position, collisionCheckRadius, collisionLayerMask);
////        foreach (var collider in colliders)
////        {
////            if (collider != objectTransform.GetComponent<Collider>())
////            {
////                Vector3 direction = position - collider.transform.position;
////                float distance = direction.magnitude;
////                Vector3 collisionPoint = collider.ClosestPoint(position);
////                if (distance < collisionCheckRadius)
////                {
////                    position = collisionPoint + direction.normalized * collisionCheckRadius;
////                }
////            }
////        }
////    }

////    private void DropObject()
////    {
////        isPickedUp = false;
////        objectTransform = null;
////    }
////}


//////using System.Collections;
//////using System.Collections.Generic;
//////using UnityEngine;

//////public class PerspectiveScale : MonoBehaviour
//////{
//////    private Camera mainCamera;
//////    private bool isPickedUp = false;
//////    private Transform objectTransform;
//////    private Vector3 initialScale;
//////    private Vector3 initialPosition;
//////    private float maxDistance = 10f;
//////    private float collisionCheckRadius = 1f;
//////    private LayerMask collisionLayerMask;

//////    private void Start()
//////    {
//////        mainCamera = Camera.main;
//////        collisionLayerMask = LayerMask.GetMask("Default"); // Adjust the layer name if needed
//////    }

//////    private void Update()
//////    {
//////        if (Input.GetKeyDown(KeyCode.T))
//////        {
//////            if (isPickedUp)
//////            {
//////                DropObject();
//////            }
//////            else
//////            {
//////                PickUpObject();
//////            }
//////        }

//////        if (isPickedUp)
//////        {
//////            ResizeObject();
//////        }
//////    }

//////    private void PickUpObject()
//////    {
//////        RaycastHit hit;
//////        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit))
//////        {
//////            float distance = Vector3.Distance(mainCamera.transform.position, hit.point);
//////            if (distance <= maxDistance)
//////            {
//////                isPickedUp = true;
//////                objectTransform = hit.transform;
//////                initialScale = objectTransform.localScale;
//////                initialPosition = objectTransform.position;
//////            }
//////        }
//////    }

//////    private void ResizeObject()
//////    {
//////        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
//////        RaycastHit hit;
//////        if (Physics.Raycast(ray, out hit))
//////        {
//////            float distance = Vector3.Distance(mainCamera.transform.position, hit.point);
//////            if (distance > maxDistance)
//////            {
//////                distance = maxDistance;
//////            }
//////            float scaleMultiplier = distance / initialScale.magnitude;
//////            objectTransform.localScale = initialScale * scaleMultiplier;

//////            Vector3 direction = hit.point - mainCamera.transform.position;
//////            Vector3 newPosition = initialPosition + direction;
//////            CheckCollision(ref newPosition);
//////            objectTransform.position = newPosition;
//////        }
//////    }

//////    private void CheckCollision(ref Vector3 position)
//////    {
//////        Collider[] colliders = Physics.OverlapSphere(position, collisionCheckRadius, collisionLayerMask);
//////        foreach (var collider in colliders)
//////        {
//////            if (collider != objectTransform.GetComponent<Collider>())
//////            {
//////                Vector3 direction = position - collider.transform.position;
//////                float distance = direction.magnitude;
//////                Vector3 collisionPoint = collider.ClosestPoint(position);
//////                if (distance < collisionCheckRadius)
//////                {
//////                    position = collisionPoint + direction.normalized * collisionCheckRadius;
//////                }
//////            }
//////        }
//////    }

//////    private void DropObject()
//////    {
//////        isPickedUp = false;
//////        objectTransform = null;
//////    }
//////}


////////using System.Collections;
////////using System.Collections.Generic;
////////using UnityEngine;

////////public class PerspectiveScale : MonoBehaviour
////////{
////////    private Camera mainCamera;
////////    private bool isPickedUp = false;
////////    private Transform objectTransform;
////////    private Vector3 initialScale;
////////    private Vector3 initialPosition;

////////    private void Start()
////////    {
////////        mainCamera = Camera.main;
////////    }

////////    private void Update()
////////    {
////////        if (Input.GetKeyDown(KeyCode.T))
////////        {
////////            if (isPickedUp)
////////            {
////////                DropObject();
////////            }
////////            else
////////            {
////////                PickUpObject();
////////            }
////////        }

////////        if (isPickedUp)
////////        {
////////            ResizeObject();
////////        }
////////    }

////////    private void PickUpObject()
////////    {
////////        RaycastHit hit;
////////        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit))
////////        {
////////            isPickedUp = true;
////////            objectTransform = hit.transform;
////////            initialScale = objectTransform.localScale;
////////            initialPosition = objectTransform.position;
////////        }
////////    }

////////    private void ResizeObject()
////////    {
////////        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
////////        RaycastHit hit;
////////        if (Physics.Raycast(ray, out hit))
////////        {
////////            Vector3 objectToCamera = mainCamera.transform.position - objectTransform.position;
////////            float distance = Vector3.Dot(objectToCamera, objectTransform.forward);
////////            float scaleMultiplier = distance / initialScale.magnitude;
////////            objectTransform.localScale = initialScale * scaleMultiplier;

////////            Vector3 direction = hit.point - mainCamera.transform.position;
////////            objectTransform.position = initialPosition + direction;
////////        }
////////    }

////////    private void DropObject()
////////    {
////////        isPickedUp = false;
////////        objectTransform = null;
////////    }
////////}






//////////using System.Collections;
//////////using System.Collections.Generic;
//////////using UnityEngine;

//////////public class PerspectiveScale : MonoBehaviour
//////////{
//////////    private Camera mainCamera;
//////////    private bool isPickedUp = false;
//////////    private Transform objectTransform;
//////////    private float objectDistance;
//////////    private float initialScale;
//////////    private Vector3 initialPosition;

//////////    private void Start()
//////////    {
//////////        mainCamera = Camera.main;
//////////    }

//////////    private void Update()
//////////    {
//////////        if (Input.GetKeyDown(KeyCode.T))
//////////        {
//////////            if (isPickedUp)
//////////            {
//////////                DropObject();
//////////            }
//////////            else
//////////            {
//////////                PickUpObject();
//////////            }
//////////        }

//////////        if (isPickedUp)
//////////        {
//////////            ResizeObject();
//////////        }
//////////    }

//////////    private void PickUpObject()
//////////    {
//////////        RaycastHit hit;
//////////        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit))
//////////        {
//////////            isPickedUp = true;
//////////            objectTransform = hit.transform;
//////////            objectDistance = Vector3.Distance(mainCamera.transform.position, objectTransform.position);
//////////            initialScale = objectTransform.localScale.x;
//////////            initialPosition = objectTransform.position;
//////////        }
//////////    }

//////////    private void ResizeObject()
//////////    {
//////////        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
//////////        RaycastHit hit;
//////////        if (Physics.Raycast(ray, out hit))
//////////        {
//////////            float distance = Vector3.Distance(mainCamera.transform.position, hit.point);
//////////            float scaleMultiplier = objectDistance / distance;
//////////            objectTransform.localScale = new Vector3(initialScale * scaleMultiplier, initialScale * scaleMultiplier, initialScale * scaleMultiplier);

//////////            Vector3 direction = hit.point - mainCamera.transform.position;
//////////            Vector3 newPosition = initialPosition + direction.normalized * objectDistance;

//////////            // Check if the new position is below the ground
//////////            RaycastHit groundHit;
//////////            if (Physics.Raycast(newPosition, -Vector3.up, out groundHit))
//////////            {
//////////                newPosition = groundHit.point + Vector3.up * (objectTransform.localScale.y * 0.5f);
//////////            }

//////////            objectTransform.position = newPosition;
//////////        }
//////////    }

//////////    private void DropObject()
//////////    {
//////////        isPickedUp = false;
//////////        objectTransform = null;
//////////    }
//////////}

