using UnityEngine;

public class HexDebug : MonoBehaviour
{
    void Start()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        HexagonMesh hexagonMesh = GetComponent<HexagonMesh>();
        
        Debug.Log($"[HexDebug.Start] === HEX DEBUG: {gameObject.name} ===");
        Debug.Log($"[HexDebug.Start] MeshFilter: " + (meshFilter ? "OK" : "Missing"));
        Debug.Log($"[HexDebug.Start] MeshRenderer: " + (meshRenderer ? "OK" : "Missing"));
        Debug.Log($"[HexDebug.Start] HexagonMesh: " + (hexagonMesh ? "OK" : "Missing"));
        Debug.Log($"[HexDebug.Start] Material: " + (meshRenderer && meshRenderer.material ? meshRenderer.material.name : "Missing"));
        
        if (meshFilter && meshFilter.mesh)
        {
            Debug.Log($"[HexDebug.Start] Vertices: {meshFilter.mesh.vertexCount}");
            Debug.Log($"[HexDebug.Start] Triangles: {meshFilter.mesh.triangles.Length / 3}");
            Debug.Log($"[HexDebug.Start] Mesh Bounds: {meshFilter.mesh.bounds}");
            Debug.Log($"[HexDebug.Start] Triangle Indices: [{string.Join(", ", meshFilter.mesh.triangles)}]");
        }
        else
        {
            Debug.Log("[HexDebug.Start] Vertices: No mesh");
        }

        // Output the world position of this hex
        Debug.Log($"[HexDebug.Start] World Position: {transform.position}");
        Debug.Log($"[HexDebug.Start] Local Scale: {transform.localScale}");
        
        Debug.Log("[HexDebug.Start] === END HEX DEBUG ===");
    }
}