using UnityEngine;

public class MainLoop : MonoBehaviour
{
    public GameObject hexTilePrefab;
    public int width = 10;
    public int height = 10;
    public float hexRadius = 1f;

    private void Start()
    {
        GameObject gridObj = new GameObject("HexGrid");
        HexGridGenerator gridGenerator = gridObj.AddComponent<HexGridGenerator>();
        gridGenerator.hexTilePrefab = hexTilePrefab;
        gridGenerator.width = width;
        gridGenerator.height = height;
        gridGenerator.hexRadius = hexRadius;
        gridGenerator.GenerateGrid();
    }
}
