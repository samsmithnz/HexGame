using UnityEngine;

public enum HexTileType
{
    Grass,
    Water
}

[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(Collider))]
public class HexTile : MonoBehaviour
{
    public HexTileType tileType = HexTileType.Grass;
    public Color grassColor = Color.green;
    public Color waterColor = Color.blue;
    public Color highlightColor = Color.yellow;

    private Renderer rend;
    private Color originalColor;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        UpdateTileAppearance();
        // Ensure a collider exists for selection
        if (GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<MeshCollider>();
        }
    }

    public void SetTileType(HexTileType type)
    {
        tileType = type;
        UpdateTileAppearance();
    }

    public void UpdateTileAppearance()
    {
        if (rend == null)
        {
            rend = GetComponent<Renderer>();
        }
        switch (tileType)
        {
            case HexTileType.Grass:
                originalColor = grassColor;
                rend.material.color = grassColor;
                break;
            case HexTileType.Water:
                originalColor = waterColor;
                rend.material.color = waterColor;
                break;
        }
    }

    public void Highlight()
    {
        if (rend == null)
        {
            rend = GetComponent<Renderer>();
        }
        rend.material.color = highlightColor;
    }

    public void RestoreColor()
    {
        if (rend == null)
        {
            rend = GetComponent<Renderer>();
        }
        rend.material.color = originalColor;
    }
}
