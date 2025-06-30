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