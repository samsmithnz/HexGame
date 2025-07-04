using UnityEngine;
using UnityEngine.UI;

public class HexSelectionManager : MonoBehaviour
{
    private HexTile selectedTile;
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
                    if (selectedTile != null && selectedTile != tile)
                    {
                        selectedTile.RestoreColor();
                    }
                    tile.Highlight();
                    selectedTile = tile;
                    UpdateInfoPanel(tile);
                }
            }
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
