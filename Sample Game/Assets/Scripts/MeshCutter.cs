//using System.Collections.Generic;
//using UnityEngine;

//public class MeshCutter : MonoBehaviour
//{
//    public GameObject objectToCut;  // Reference to the object you want to cut in the Unity Editor

//    void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.Space))
//        {
//            CutObject();
//        }
//    }

//    void CutObject()
//    {
//        // Create a plane to cut the object in half
//        Plane cutPlane = new Plane(transform.up, transform.position);

//        // Get the mesh of the object to cut
//        Mesh objectMesh = objectToCut.GetComponent<MeshFilter>().mesh;

//        // Create arrays to hold the vertices of the two resulting meshes
//        Vector3[] verticesA, verticesB;
//        int[] trianglesA, trianglesB;

//        // Perform the actual cut
//        MeshCut.Cut(objectMesh, cutPlane, out verticesA, out verticesB, out trianglesA, out trianglesB);

//        // Create new game objects and meshes for the two halves
//        GameObject half1 = new GameObject("Half 1");
//        MeshFilter meshFilter1 = half1.AddComponent<MeshFilter>();
//        MeshRenderer meshRenderer1 = half1.AddComponent<MeshRenderer>();
//        Mesh halfMesh1 = new Mesh();
//        halfMesh1.vertices = verticesA;
//        halfMesh1.triangles = trianglesA;
//        meshFilter1.mesh = halfMesh1;
//        meshRenderer1.material = objectToCut.GetComponent<MeshRenderer>().material;

//        GameObject half2 = new GameObject("Half 2");
//        MeshFilter meshFilter2 = half2.AddComponent<MeshFilter>();
//        MeshRenderer meshRenderer2 = half2.AddComponent<MeshRenderer>();
//        Mesh halfMesh2 = new Mesh();
//        halfMesh2.vertices = verticesB;
//        halfMesh2.triangles = trianglesB;
//        meshFilter2.mesh = halfMesh2;
//        meshRenderer2.material = objectToCut.GetComponent<MeshRenderer>().material;

//        // Set the position and rotation of the halves to match the original object
//        half1.transform.position = objectToCut.transform.position;
//        half1.transform.rotation = objectToCut.transform.rotation;
//        half2.transform.position = objectToCut.transform.position;
//        half2.transform.rotation = objectToCut.transform.rotation;

//        // Destroy the original object
//        Destroy(objectToCut);
//    }
//}

//public static class MeshCut
//{
//    public static void Cut(Mesh mesh, Plane cutPlane, out Vector3[] verticesA, out Vector3[] verticesB,
//                           out int[] trianglesA, out int[] trianglesB)
//    {
//        Vector3[] vertices = mesh.vertices;
//        int[] triangles = mesh.triangles;

//        // Lists to hold the vertices and triangles for the two resulting meshes
//        List<Vector3> verticesListA = new List<Vector3>();
//        List<Vector3> verticesListB = new List<Vector3>();
//        List<int> trianglesListA = new List<int>();
//        List<int> trianglesListB = new List<int>();

//        // Classify each vertex of the mesh based on its position relative to the cut plane
//        bool[] vertexIsAbovePlane = new bool[vertices.Length];
//        for (int i = 0; i < vertices.Length; i++)
//        {
//            vertexIsAbovePlane[i] = cutPlane.GetSide(vertices[i]);
//        }

//        // Iterate over each triangle of the mesh
//        for (int i = 0; i < triangles.Length; i += 3)
//        {
//            int vertexIndexA = triangles[i];
//            int vertexIndexB = triangles[i + 1];
//            int vertexIndexC = triangles[i + 2];

//            bool abovePlaneA = vertexIsAbovePlane[vertexIndexA];
//            bool abovePlaneB = vertexIsAbovePlane[vertexIndexB];
//            bool abovePlaneC = vertexIsAbovePlane[vertexIndexC];

//            // Classify the triangle based on the positions of its vertices relative to the cut plane
//            if (abovePlaneA && abovePlaneB && abovePlaneC)
//            {
//                // Triangle is completely above the cut plane, add it to mesh A
//                trianglesListA.Add(vertexIndexA);
//                trianglesListA.Add(vertexIndexB);
//                trianglesListA.Add(vertexIndexC);
//            }
//            else if (!abovePlaneA && !abovePlaneB && !abovePlaneC)
//            {
//                // Triangle is completely below the cut plane, add it to mesh B
//                trianglesListB.Add(vertexIndexA);
//                trianglesListB.Add(vertexIndexB);
//                trianglesListB.Add(vertexIndexC);
//            }
//            else
//            {
//                // Triangle intersects the cut plane, split it into two new triangles and distribute them to both meshes
//                Vector3 intersectionPointA, intersectionPointB;
//                int intersectionIndexA, intersectionIndexB;

//                if (abovePlaneA != abovePlaneB)
//                {
//                    cutPlane.Raycast(new Ray(vertices[vertexIndexA], (vertices[vertexIndexB] - vertices[vertexIndexA]).normalized),
//                                     out float distance);
//                    intersectionPointA = vertices[vertexIndexA] + (vertices[vertexIndexB] - vertices[vertexIndexA]).normalized * distance;
//                    intersectionIndexA = verticesListA.Count;
//                    verticesListA.Add(intersectionPointA);

//                    intersectionPointB = vertices[vertexIndexA] + (vertices[vertexIndexB] - vertices[vertexIndexA]).normalized * distance;
//                    intersectionIndexB = verticesListB.Count;
//                    verticesListB.Add(intersectionPointB);
//                }
//                else if (abovePlaneB != abovePlaneC)
//                {
//                    cutPlane.Raycast(new Ray(vertices[vertexIndexB], (vertices[vertexIndexC] - vertices[vertexIndexB]).normalized),
//                                     out float distance);
//                    intersectionPointA = vertices[vertexIndexB] + (vertices[vertexIndexC] - vertices[vertexIndexB]).normalized * distance;
//                    intersectionIndexA = verticesListA.Count;
//                    verticesListA.Add(intersectionPointA);

//                    intersectionPointB = vertices[vertexIndexB] + (vertices[vertexIndexC] - vertices[vertexIndexB]).normalized * distance;
//                    intersectionIndexB = verticesListB.Count;
//                    verticesListB.Add(intersectionPointB);
//                }
//                else // abovePlaneA != abovePlaneC
//                {
//                    cutPlane.Raycast(new Ray(vertices[vertexIndexA], (vertices[vertexIndexC] - vertices[vertexIndexA]).normalized),
//                    out float distance);
//                    intersectionPointA = vertices[vertexIndexA] + (vertices[vertexIndexC] - vertices[vertexIndexA]).normalized * distance;
//                    intersectionIndexA = verticesListA.Count;
//                    verticesListA.Add(intersectionPointA);
//                    intersectionPointB = vertices[vertexIndexA] + (vertices[vertexIndexC] - vertices[vertexIndexA]).normalized * distance;
//                    intersectionIndexB = verticesListB.Count;
//                    verticesListB.Add(intersectionPointB);
//                }

//                // Add the new triangles to the respective lists
//                if (abovePlaneA)
//                {
//                    trianglesListA.Add(vertexIndexA);
//                    trianglesListA.Add(intersectionIndexA);
//                    trianglesListA.Add(intersectionIndexB);

//                    trianglesListB.Add(intersectionIndexA);
//                    trianglesListB.Add(vertexIndexB);
//                    trianglesListB.Add(vertexIndexC);
//                }
//                else
//                {
//                    trianglesListA.Add(intersectionIndexA);
//                    trianglesListA.Add(vertexIndexB);
//                    trianglesListA.Add(vertexIndexC);

//                    trianglesListB.Add(vertexIndexA);
//                    trianglesListB.Add(intersectionIndexA);
//                    trianglesListB.Add(intersectionIndexB);
//                }
//            }
//        }

//        // Convert the lists to arrays
//        verticesA = verticesListA.ToArray();
//        verticesB = verticesListB.ToArray();
//        trianglesA = trianglesListA.ToArray();
//        trianglesB = trianglesListB.ToArray();
//    }
//}

