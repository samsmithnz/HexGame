using UnityEngine;

public class HexDebug : MonoBehaviour
{
    void Start()
    {
        var meshFilter = GetComponent<MeshFilter>();
        var meshRenderer = GetComponent<MeshRenderer>();
        var hexagonMesh = GetComponent<HexagonMesh>();
        
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
        }
        else
        {
            Debug.Log("Vertices: No mesh");
        }

        // Output the world position of this hex
        Debug.Log($"World Position: {transform.position}");
        Debug.Log($"Local Scale: {transform.localScale}");
        
        // Report HexagonMesh settings if available
        if (hexagonMesh)
        {
            Debug.Log($"Generate Border: {hexagonMesh.generateBorder}");
            Debug.Log($"Border Thickness: {hexagonMesh.borderThickness}");
        }
        
        Debug.Log("=== END HEX DEBUG ===");
    }
}