using UnityEngine;

public class MainLoop : MonoBehaviour
{
    public GameObject hexTilePrefab;
    public int width = 10;
    public int height = 10;
    [Header("Centralized Settings")]
    public float hexRadius = 1f; // Central source of truth
    public Material defaultMaterial;

    private void Start()
    {
        GameObject gridObj = new GameObject("HexGrid");
        HexGridGenerator gridGenerator = gridObj.AddComponent<HexGridGenerator>();
        gridGenerator.hexTilePrefab = hexTilePrefab;
        gridGenerator.width = width;
        gridGenerator.height = height;
        gridGenerator.hexRadius = hexRadius; // Pass the centralized value
        gridGenerator.defaultMaterial = defaultMaterial; // Pass the default material
        gridGenerator.GenerateGrid();
    }
}
