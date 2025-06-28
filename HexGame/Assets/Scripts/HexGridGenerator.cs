using UnityEngine;

public class HexGridGenerator : MonoBehaviour
{
    public GameObject hexTilePrefab;
    public int width = 10;
    public int height = 10;
    public float hexRadius = 1f;

    public void GenerateGrid()
    {
        float hexWidth = hexRadius * 2f;
        float hexHeight = Mathf.Sqrt(3f) * hexRadius;
        float xOffset = hexWidth * 0.75f;
        float zOffset = hexHeight;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xPos = x * xOffset;
                float zPos = y * zOffset + (x % 2 == 0 ? 0 : zOffset / 2f);
                Vector3 pos = new Vector3(xPos, 0, zPos);
                GameObject tile = Instantiate(hexTilePrefab, pos, Quaternion.identity, this.transform);
                tile.name = $"HexTile_{x}_{y}";
            }
        }
    }
}
