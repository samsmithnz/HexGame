using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexSelectionManager : MonoBehaviour
{
    private HexTile selectedTile;
    private List<HexTile> highlightedTiles = new List<HexTile>();
    private static HexSelectionManager instance;

    // UI
    private GameObject infoPanel;
    private Text infoText;

    void Awake()
    {
        instance = this;
        CreateInfoPanel();
    }

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
                    // If clicking a highlighted attack tile, perform attack
                    if (highlightedTiles.Contains(tile))
                    {
                        AttackTile(tile);
                        return;
                    }

                    // Only allow selecting a tile if:
                    // - It is owned by the current player
                    // - It has 2 or more armies
                    if (tile.hexColor == GameManager.Instance.currentPlayer && tile.armyCount >= 2)
                    {
                        // Clear previous selection and highlights
                        if (selectedTile != null)
                        {
                            selectedTile.RestoreColor();
                        }
                        ClearHighlights();
                        
                        // Select new tile
                        tile.Highlight();
                        selectedTile = tile;
                        UpdateInfoPanel(tile);
                        HighlightAttackableNeighbors(tile);
                    }
                    else
                    {
                        // Deselect if clicking an invalid tile
                        if (selectedTile != null)
                        {
                            selectedTile.RestoreColor();
                            selectedTile = null;
                        }
                        ClearHighlights();
                        UpdateInfoPanel(tile);
                    }
                }
            }
        }
    }

    private void HighlightAttackableNeighbors(HexTile tile)
    {
        // Find neighbors using tile grid positions
        List<HexTile> neighbors = GetNeighbors(tile);
        foreach (HexTile neighbor in neighbors)
        {
            // Highlight if unowned or owned by opponent
            if (neighbor.hexColor == HexColor.None || neighbor.hexColor != GameManager.Instance.currentPlayer)
            {
                neighbor.HighlightAttack();
                highlightedTiles.Add(neighbor);
            }
        }
    }

    private void ClearHighlights()
    {
        foreach (HexTile tile in highlightedTiles)
        {
            tile.RestoreColor();
        }
        highlightedTiles.Clear();
    }

    private List<HexTile> GetNeighbors(HexTile tile)
    {
        List<HexTile> neighbors = new List<HexTile>();
        // Parse tile name as HexTile_x_y
        string[] parts = tile.name.Split('_');
        int x, y;
        if (parts.Length == 3 && int.TryParse(parts[1], out x) && int.TryParse(parts[2], out y))
        {
            // Use correct neighbor offsets for even-q vertical layout
            (int dx, int dy)[] evenOffsets = new (int, int)[] { (1, 0), (-1, 0), (0, 1), (0, -1), (1, -1), (-1, -1) };
            (int dx, int dy)[] oddOffsets = new (int, int)[] { (1, 0), (-1, 0), (0, 1), (0, -1), (1, 1), (-1, 1) };
            (int dx, int dy)[] neighborOffsets = (x % 2 == 0) ? evenOffsets : oddOffsets;
            foreach ((int dx, int dy) in neighborOffsets)
            {
                int nx = x + dx;
                int ny = y + dy;
                string neighborName = $"HexTile_{nx}_{ny}";
                foreach (HexTile t in GameManager.Instance.GetAllTiles())
                {
                    if (t.name == neighborName)
                    {
                        neighbors.Add(t);
                        break;
                    }
                }
            }
        }
        return neighbors;
    }

    private void AttackTile(HexTile target)
    {
        if (selectedTile == null)
        {
            return;
        }
        
        // Move all but one army to the target
        int movingArmies = selectedTile.armyCount - 1;
        if (movingArmies < 1)
        {
            return;
        }
        
        // Set target to current player with moving armies
        target.SetHexColor(GameManager.Instance.currentPlayer, movingArmies);
        
        // Leave one army on source
        selectedTile.armyCount = 1;
        selectedTile.UpdateTileAppearance();
        
        // Update visuals and UI
        GameManager.Instance.CallUpdateAllArmyVisuals();
        GameManager.Instance.CallUpdateUI();
        
        // Check if the new tile can attack again
        if (target.armyCount >= 2)
        {
            // Deselect old, select new
            selectedTile.RestoreColor();
            selectedTile = target;
            selectedTile.Highlight();
            ClearHighlights();
            HighlightAttackableNeighbors(selectedTile);
            UpdateInfoPanel(selectedTile);
        }
        else
        {
            // Deselect and clear highlights
            selectedTile.RestoreColor();
            selectedTile = null;
            ClearHighlights();
            UpdateInfoPanel(target);
        }
    }

    private void CreateInfoPanel()
    {
        // Only create a new Canvas if one does not already exist
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("HexInfoCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }
        // Only create the info panel if it doesn't already exist
        if (canvas.transform.Find("HexInfoPanel") != null)
        {
            infoPanel = canvas.transform.Find("HexInfoPanel").gameObject;
            infoText = infoPanel.GetComponentInChildren<Text>();
            return;
        }
        infoPanel = new GameObject("HexInfoPanel");
        infoPanel.transform.SetParent(canvas.transform, false);
        Image panelImage = infoPanel.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.5f);
        RectTransform panelRect = infoPanel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0, 1);
        panelRect.anchorMax = new Vector2(0, 1);
        panelRect.pivot = new Vector2(0, 1);
        panelRect.anchoredPosition = new Vector2(10, -10);
        panelRect.sizeDelta = new Vector2(220, 80);
        // Create Text
        GameObject textObj = new GameObject("HexInfoText");
        textObj.transform.SetParent(infoPanel.transform, false);
        infoText = textObj.AddComponent<Text>();
        infoText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        infoText.fontSize = 16;
        infoText.color = Color.white;
        infoText.alignment = TextAnchor.UpperLeft;
        RectTransform textRect = infoText.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(10, 10);
        textRect.offsetMax = new Vector2(-10, -10);
        infoText.text = "No tile selected";
    }

    private void UpdateInfoPanel(HexTile tile)
    {
        string coords = tile.name;
        string color = tile.hexColor.ToString();
        int armies = tile.armyCount;
        infoText.text = $"Tile: {coords}\nColor: {color}\nArmies: {armies}";
    }
}
