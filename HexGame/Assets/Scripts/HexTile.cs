using UnityEngine;

[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(Collider))]
public class HexTile : MonoBehaviour
{
    public HexColor hexColor = HexColor.None;
    public Color noneColor = Color.gray;
    public Color blueColor = Color.blue;
    public Color greenColor = Color.green;
    public Color highlightColor = Color.yellow;

    public int armyCount = 0;

    private Renderer rend;
    private Color originalColor;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        UpdateTileAppearance();
        if (GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<MeshCollider>();
        }
    }

    public void SetHexColor(HexColor color, int armies)
    {
        hexColor = color;
        armyCount = armies;
        UpdateTileAppearance();
    }

    public void UpdateTileAppearance()
    {
        if (rend == null)
        {
            rend = GetComponent<Renderer>();
        }
        switch (hexColor)
        {
            case HexColor.Blue:
                originalColor = blueColor;
                rend.material.color = blueColor;
                break;
            case HexColor.Green:
                originalColor = greenColor;
                rend.material.color = greenColor;
                break;
            default:
                originalColor = noneColor;
                rend.material.color = noneColor;
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
        UpdateTileAppearance();
    }
}
