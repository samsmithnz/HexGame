using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexagonMesh : MonoBehaviour
{
    public Material defaultMaterial;

    [Header("Border Settings")]
    public bool generateBorder = true;
    public float borderThickness = 0.1f;
    public Material borderMaterial;

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
        if (generateBorder && borderMaterial != null)
        {
            CreateHexWithBorder(mesh);
        }
        else
        {
            CreateSimpleHex(mesh);
        }
        GetComponent<MeshFilter>().mesh = mesh;
    }

    private void CreateSimpleHex(Mesh mesh)
    {
        Vector3[] vertices = new Vector3[7];
        vertices[0] = Vector3.zero;
        for (int i = 0; i < 6; i++)
        {
            float angle = i * 60f * Mathf.Deg2Rad;
            vertices[i + 1] = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * hexRadius;
        }
        int[] triangles = new int[18];
        for (int i = 0; i < 6; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = (i + 1) % 6 + 1;
            triangles[i * 3 + 2] = i + 1;
        }
        Vector2[] uvs = new Vector2[7];
        uvs[0] = new Vector2(0.5f, 0.5f);
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
    }

    private void CreateHexWithBorder(Mesh mesh)
    {
        float innerRadius = hexRadius;
        float outerRadius = hexRadius + borderThickness;
        Vector3[] vertices = new Vector3[13];

        // Center
        vertices[0] = Vector3.zero;
        // Inner ring
        for (int i = 0; i < 6; i++)
        {
            float angle = i * 60f * Mathf.Deg2Rad;
            vertices[i + 1] = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * innerRadius;
        }
        // Outer ring
        for (int i = 0; i < 6; i++)
        {
            float angle = i * 60f * Mathf.Deg2Rad;
            vertices[i + 7] = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * outerRadius;
        }

        // Triangles: center to inner ring
        int[] triangles = new int[6 * 3 + 6 * 6];
        int t = 0;
        for (int i = 0; i < 6; i++)
        {
            triangles[t++] = 0;
            triangles[t++] = (i + 1);
            triangles[t++] = (i + 1) % 6 + 1;
        }
        // Border quads
        for (int i = 0; i < 6; i++)
        {
            int innerA = i + 1;
            int innerB = (i + 1) % 6 + 1;
            int outerA = i + 7;
            int outerB = (i + 1) % 6 + 7;

            triangles[t++] = innerA;
            triangles[t++] = outerA;
            triangles[t++] = outerB;

            triangles[t++] = innerA;
            triangles[t++] = outerB;
            triangles[t++] = innerB;
        }

        // UVs
        Vector2[] uvs = new Vector2[13];
        uvs[0] = new Vector2(0.5f, 0.5f);
        for (int i = 0; i < 6; i++)
        {
            float angle = i * 60f * Mathf.Deg2Rad;
            uvs[i + 1] = new Vector2(0.5f + Mathf.Cos(angle) * 0.5f * innerRadius / outerRadius, 0.5f + Mathf.Sin(angle) * 0.5f * innerRadius / outerRadius);
            uvs[i + 7] = new Vector2(0.5f + Mathf.Cos(angle) * 0.5f, 0.5f + Mathf.Sin(angle) * 0.5f);
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
    }

    private void EnsureMaterial()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (generateBorder && borderMaterial != null)
        {
            renderer.sharedMaterial = borderMaterial;
        }
        else if (renderer.sharedMaterial == null)
        {
            if (defaultMaterial == null)
            {
                defaultMaterial = new Material(Shader.Find("Standard"));
                defaultMaterial.color = Color.gray;
            }
            renderer.sharedMaterial = defaultMaterial;
        }
    }

    public void Regenerate()
    {
        CreateHexagonMesh();
        EnsureMaterial();
    }
}