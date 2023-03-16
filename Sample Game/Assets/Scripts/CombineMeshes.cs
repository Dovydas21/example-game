using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

//[RequireComponent(typeof(MeshRenderer))]

public class CombineMeshes : MonoBehaviour
{
    private MeshFilter targetMeshFilter;
    public bool addCollider;

    public void Awake()
    {
        ArrayList materials = new ArrayList();
        ArrayList combineInstanceArrays = new ArrayList();
        MeshFilter[] meshFilters = gameObject.GetComponentsInChildren<MeshFilter>();

        foreach (MeshFilter meshFilter in meshFilters)
        {
            MeshRenderer meshRenderer = meshFilter.GetComponent<MeshRenderer>();

            if (!meshRenderer ||
                !meshFilter.sharedMesh ||
                meshRenderer.sharedMaterials.Length != meshFilter.sharedMesh.subMeshCount)
            {
                continue;
            }

            for (int s = 0; s < meshFilter.sharedMesh.subMeshCount; s++)
            {
                int materialArrayIndex = Contains(materials, meshRenderer.sharedMaterials[s].name);
                if (materialArrayIndex == -1)
                {
                    materials.Add(meshRenderer.sharedMaterials[s]);
                    materialArrayIndex = materials.Count - 1;
                }
                combineInstanceArrays.Add(new ArrayList());

                CombineInstance combineInstance = new CombineInstance();
                combineInstance.transform = meshRenderer.transform.localToWorldMatrix;
                combineInstance.subMeshIndex = s;
                combineInstance.mesh = meshFilter.sharedMesh;
                (combineInstanceArrays[materialArrayIndex] as ArrayList).Add(combineInstance);
            }
        }

        // Get / Create mesh filter & renderer
        MeshFilter meshFilterCombine = gameObject.GetComponent<MeshFilter>();
        if (meshFilterCombine == null)
        {
            meshFilterCombine = gameObject.AddComponent<MeshFilter>();
        }
        MeshRenderer meshRendererCombine = gameObject.GetComponent<MeshRenderer>();
        if (meshRendererCombine == null)
        {
            meshRendererCombine = gameObject.AddComponent<MeshRenderer>();
        }

        // Combine by material index into per-material meshes
        // also, Create CombineInstance array for next step
        Mesh[] meshes = new Mesh[materials.Count];
        CombineInstance[] combineInstances = new CombineInstance[materials.Count];

        for (int m = 0; m < materials.Count; m++)
        {
            CombineInstance[] combineInstanceArray = (combineInstanceArrays[m] as ArrayList).ToArray(typeof(CombineInstance)) as CombineInstance[];
            meshes[m] = new Mesh();
            meshes[m].indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            meshes[m].CombineMeshes(combineInstanceArray, true, true);

            combineInstances[m] = new CombineInstance();
            combineInstances[m].mesh = meshes[m];
            combineInstances[m].subMeshIndex = 0;
        }

        // Combine into one
        meshFilterCombine.sharedMesh = new Mesh();
        meshFilterCombine.sharedMesh.CombineMeshes(combineInstances, false, false);
        FixPos();

        if (addCollider)
        {
            MeshCollider mc = gameObject.AddComponent<MeshCollider>();
            mc.sharedMesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
        }

        // Assign materials
        Material[] materialsArray = materials.ToArray(typeof(Material)) as Material[];
        meshRendererCombine.materials = materialsArray;


        // Delete the child GameObjects to reduce draw calls and time spent on the render thread.
        int childCount = transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            print("CombineMeshes.cs: About to delete child: " + transform.GetChild(i).name);
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    private int Contains(ArrayList searchList, string searchName)
    {
        for (int i = 0; i < searchList.Count; i++)
        {
            if (((Material)searchList[i]).name == searchName)
            {
                return i;
            }
        }
        return -1;
    }

    void FixPos()
    {
        // Set the position of the combined mesh to the position of the original objects.
        transform.localScale = new Vector3(1f, 1f, 1f);
        transform.rotation = Quaternion.identity;
        transform.position = Vector3.zero;
    }
}