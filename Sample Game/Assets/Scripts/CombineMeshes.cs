using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]

public class CombineMeshes : MonoBehaviour
{
    private MeshFilter targetMeshFilter;

    private void Awake()
    {
        Combine();
    }

    private void Combine()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        targetMeshFilter = gameObject.AddComponent<MeshFilter>();
        var combine = new CombineInstance[meshFilters.Length];

        for (var i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
        }

        // Create a new mesh and set it so that it can have more than the default number of verts. (65k = default @ UInt16)
        var mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        // Combine the meshes into one mesh and set the mesh filter to the new mesh.
        mesh.CombineMeshes(combine);
        targetMeshFilter.mesh = mesh;
        gameObject.SetActive(true); // Enable the current object so it can be seen.

        // Set the position of the combined mesh to the position of the original objects.
        transform.localScale = new Vector3(1f, 1f, 1f);
        transform.rotation = Quaternion.identity;
        transform.position = Vector3.zero;
    }
}