using UnityEngine;

public class HexGridGenerator : MonoBehaviour
{
    public GameObject hexTilePrefab;
    public int width = 10;
    public int height = 10;
    [Header("Centralized Settings")]
    public float hexRadius = 1f; // ONLY place radius is defined
    public float borderThickness = 0.1f;

    public void GenerateGrid()
    {
        // Clear any existing tiles
        foreach (Transform child in transform)
        {
            if (Application.isPlaying)
                Destroy(child.gameObject);
            else
                DestroyImmediate(child.gameObject);
        }

        // Correct hexagon tiling mathematics
        float horizontalSpacing = hexRadius * 1.5f;
        float verticalSpacing = hexRadius * Mathf.Sqrt(3f);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xPos = x * horizontalSpacing;
                float zPos = y * verticalSpacing;
                
                // Offset every other column
                if (x % 2 == 1)
                {
                    zPos += verticalSpacing * 0.5f;
                }
                
                Vector3 pos = new Vector3(xPos, 0, zPos);
                GameObject tile = Instantiate(hexTilePrefab, pos, Quaternion.identity, this.transform);
                tile.name = $"HexTile_{x}_{y}";

                // Set centralized values on all components
                HexagonMesh mesh = tile.GetComponent<HexagonMesh>();
                if (mesh != null)
                {
                    mesh.SetRadius(hexRadius);
                    mesh.borderThickness = borderThickness;
                    mesh.Regenerate();
                }
            }
        }
    }
}
