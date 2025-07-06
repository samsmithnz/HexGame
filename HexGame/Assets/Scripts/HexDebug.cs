using UnityEngine;

public class HexDebug : MonoBehaviour
{
    void Start()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        HexagonMesh hexagonMesh = GetComponent<HexagonMesh>();
        
        Debug.Log($"=== HEX DEBUG: {gameObject.name} ===");
        Debug.Log("MeshFilter: " + (meshFilter ? "OK" : "Missing"));
        Debug.Log("MeshRenderer: " + (meshRenderer ? "OK" : "Missing"));
        Debug.Log("HexagonMesh: " + (hexagonMesh ? "OK" : "Missing"));
        Debug.Log("Material: " + (meshRenderer && meshRenderer.material ? meshRenderer.material.name : "Missing"));
        
        if (meshFilter && meshFilter.mesh)
        {
            Debug.Log($"Vertices: {meshFilter.mesh.vertexCount}");
            Debug.Log($"Triangles: {meshFilter.mesh.triangles.Length / 3}");
            Debug.Log($"Mesh Bounds: {meshFilter.mesh.bounds}");
            Debug.Log($"Triangle Indices: [{string.Join(", ", meshFilter.mesh.triangles)}]");
        }
        else
        {
            Debug.Log("Vertices: No mesh");
        }

        // Output the world position of this hex
        Debug.Log($"World Position: {transform.position}");
        Debug.Log($"Local Scale: {transform.localScale}");
        
        Debug.Log("=== END HEX DEBUG ===");
    }
}