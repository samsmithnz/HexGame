using UnityEngine;

public class HexDebug : MonoBehaviour
{
    void Start()
    {
        var meshFilter = GetComponent<MeshFilter>();
        var meshRenderer = GetComponent<MeshRenderer>();
        Debug.Log("MeshFilter: " + (meshFilter ? "OK" : "Missing"));
        Debug.Log("MeshRenderer: " + (meshRenderer ? "OK" : "Missing"));
        Debug.Log("Material: " + (meshRenderer && meshRenderer.material ? meshRenderer.material.name : "Missing"));
        Debug.Log("Vertices: " + (meshFilter && meshFilter.mesh ? meshFilter.mesh.vertexCount.ToString() : "No mesh"));
        if (meshFilter && meshFilter.mesh)
        {
            Debug.Log("Assigned mesh with " + meshFilter.mesh.vertexCount + " vertices and " + meshFilter.mesh.triangles.Length / 3 + " triangles");
        }
    }
}