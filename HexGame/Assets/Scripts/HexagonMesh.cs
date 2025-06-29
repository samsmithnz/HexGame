using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexagonMesh : MonoBehaviour
{
    private void Start()
    {
        CreateHexagonMesh();
    }

    private void CreateHexagonMesh()
    {
        Mesh mesh = new Mesh();
        
        // Hexagon vertices (6 points around a circle + center)
        Vector3[] vertices = new Vector3[7];
        vertices[0] = Vector3.zero; // Center
        
        for (int i = 0; i < 6; i++)
        {
            float angle = i * 60f * Mathf.Deg2Rad;
            vertices[i + 1] = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
        }
        
        // Triangles (6 triangles from center to each edge)
        int[] triangles = new int[18];
        for (int i = 0; i < 6; i++)
        {
            triangles[i * 3] = 0; // Center
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = (i + 1) % 6 + 1;
        }
        
        // UV coordinates
        Vector2[] uvs = new Vector2[7];
        uvs[0] = new Vector2(0.5f, 0.5f); // Center
        for (int i = 0; i < 6; i++)
        {
            float angle = i * 60f * Mathf.Deg2Rad;
            uvs[i + 1] = new Vector2(
                0.5f + Mathf.Cos(angle) * 0.5f,
                0.5f + Mathf.Sin(angle) * 0.5f
            );
        }
        
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        
        GetComponent<MeshFilter>().mesh = mesh;
    }
}