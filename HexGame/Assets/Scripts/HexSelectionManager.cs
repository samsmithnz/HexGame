using UnityEngine;

public class HexSelectionManager : MonoBehaviour
{
    private HexTile selectedTile;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                HexTile tile = hit.collider.GetComponent<HexTile>();
                if (tile != null)
                {
                    if (selectedTile != null && selectedTile != tile)
                    {
                        selectedTile.RestoreColor();
                    }
                    tile.Highlight();
                    selectedTile = tile;
                }
            }
        }
    }
}
