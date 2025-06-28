using UnityEngine;

public enum HexTileType
{
    Grass,
    Water
}

[RequireComponent(typeof(Renderer))]
public class HexTile : MonoBehaviour
{
    public HexTileType tileType = HexTileType.Grass;
    public Color grassColor = Color.green;
    public Color waterColor = Color.blue;

    private Renderer rend;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        UpdateTileAppearance();
    }

    public void SetTileType(HexTileType type)
    {
        tileType = type;
        UpdateTileAppearance();
    }

    private void UpdateTileAppearance()
    {
        if (rend == null)
        {
            rend = GetComponent<Renderer>();
        }
        switch (tileType)
        {
            case HexTileType.Grass:
                rend.material.color = grassColor;
                break;
            case HexTileType.Water:
                rend.material.color = waterColor;
                break;
        }
    }
}
