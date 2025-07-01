using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexagonMesh : MonoBehaviour
{
    public Material defaultMaterial;

    // Private - only set by grid generator
    private float hexRadius = 1f;

    private void Awake()
    {
        CreateHexagonMesh();
        EnsureMaterial();
    }

    public void SetRadius(float radius)
    {
        hexRadius = radius;
    }

    private void CreateHexagonMesh()
    {
        Mesh mesh = new Mesh();
        CreateSimpleHex(mesh);
        GetComponent<MeshFilter>().mesh = mesh;
    }

    private void CreateSimpleHex(Mesh mesh)
    {
        float height = hexRadius * Random.Range(0.1f, 0.2f); // 10-20% of radius
        Vector3[] vertices = new Vector3[14];
        // Bottom center
        vertices[0] = Vector3.zero;
        // Bottom ring
        for (int i = 0; i < 6; i++)
        {
            float angle = i * 60f * Mathf.Deg2Rad;
            vertices[i + 1] = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * hexRadius;
        }
        // Top center
        vertices[7] = new Vector3(0, height, 0);
        // Top ring
        for (int i = 0; i < 6; i++)
        {
            float angle = i * 60f * Mathf.Deg2Rad;
            vertices[i + 8] = new Vector3(Mathf.Cos(angle), height, Mathf.Sin(angle)) * hexRadius;
        }

        // Triangles: bottom face (0-6)
        int[] triangles = new int[6 * 3 + 6 * 3 + 6 * 6]; // bottom, top, sides
        int t = 0;
        // Bottom face (reverse winding)
        for (int i = 0; i < 6; i++)
        {
            triangles[t++] = 0;
            triangles[t++] = i + 1;
            triangles[t++] = (i + 1) % 6 + 1;
        }
        // Top face (reverse winding)
        for (int i = 0; i < 6; i++)
        {
            triangles[t++] = 7;
            triangles[t++] = (i + 1) % 6 + 8;
            triangles[t++] = i + 8;
        }
        // Sides (reverse winding)
        for (int i = 0; i < 6; i++)
        {
            int b0 = i + 1;
            int b1 = (i + 1) % 6 + 1;
            int t0 = i + 8;
            int t1 = (i + 1) % 6 + 8;
            // First triangle
            triangles[t++] = b0;
            triangles[t++] = t1;
            triangles[t++] = b1;
            // Second triangle
            triangles[t++] = b0;
            triangles[t++] = t0;
            triangles[t++] = t1;
        }

        Vector2[] uvs = new Vector2[14];
        uvs[0] = new Vector2(0.5f, 0.5f);
        for (int i = 0; i < 6; i++)
        {
            float angle = i * 60f * Mathf.Deg2Rad;
            uvs[i + 1] = new Vector2(0.5f + Mathf.Cos(angle) * 0.5f, 0.5f + Mathf.Sin(angle) * 0.5f);
        }
        uvs[7] = new Vector2(0.5f, 0.5f);
        for (int i = 0; i < 6; i++)
        {
            float angle = i * 60f * Mathf.Deg2Rad;
            uvs[i + 8] = new Vector2(0.5f + Mathf.Cos(angle) * 0.5f, 0.5f + Mathf.Sin(angle) * 0.5f);
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
    }

    private void EnsureMaterial()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer.sharedMaterial == null)
        {
            if (defaultMaterial == null)
            {
                defaultMaterial = new Material(Shader.Find("Standard"));
                defaultMaterial.color = Color.black;
            }
            renderer.sharedMaterial = defaultMaterial;
        }
        else if (renderer.sharedMaterial.HasProperty("_Color"))
        {
            renderer.sharedMaterial.color = Color.black;
        }
    }

    public void Regenerate()
    {
        CreateHexagonMesh();
        EnsureMaterial();
    }
}