using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HexSelectionManager : MonoBehaviour
{
    private HexTile selectedTile;
    private List<HexTile> highlightedTiles = new List<HexTile>();
    private static HexSelectionManager instance;

    // UI
    private GameObject infoPanel;
    private Text infoText;

    // Battle result popup
    private GameObject battleResultPopup;
    private TextMeshProUGUI battleResultText;
    private Coroutine battleResultCoroutine;

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
        
        // Attacker uses all but one army
        int attackerArmies = selectedTile.armyCount - 1;
        if (attackerArmies < 1)
        {
            return;
        }
        
        // Defender uses all armies (0 if unoccupied)
        int defenderArmies = target.armyCount;
        
        // Resolve combat using Risk-inspired dice system
        CombatResolver.CombatResult result = CombatResolver.ResolveCombat(attackerArmies, defenderArmies);
        
        // Show popup with battle results only if defender had armies
        if (defenderArmies > 0)
        {
            string attackerColorHex = GameManager.Instance.currentPlayer == HexColor.Blue ? "#2196F3" : "#43A047";
            string defenderColorHex = GameManager.Instance.currentPlayer == HexColor.Blue ? "#43A047" : "#2196F3";
            string resultLine;
            if (result.attackerWins)
            {
                resultLine = $"<color={attackerColorHex}>{GameManager.Instance.currentPlayer} wins!</color>";
            }
            else
            {
                // Defender color is the opposite of current player
                HexColor defenderColor = GameManager.Instance.currentPlayer == HexColor.Blue ? HexColor.Green : HexColor.Blue;
                string defenderName = defenderColor.ToString();
                resultLine = $"<color={defenderColorHex}>{defenderName} holds!</color>";
            }
            string battleMsg = $"Attacker: {attackerArmies} vs Defender: {defenderArmies}\n" +
                $"Attacker rolls: [{string.Join(", ", result.attackerRolls)}]  Defender rolls: [{string.Join(", ", result.defenderRolls)}]\n" +
                $"Attacker survivors: {result.attackerSurvivors}  Defender survivors: {result.defenderSurvivors}\n" +
                resultLine;
            ShowBattleResultPopup(battleMsg);
        }
        
        if (result.attackerWins)
        {
            // Attacker wins: take the tile with surviving armies
            target.SetHexColor(GameManager.Instance.currentPlayer, result.attackerSurvivors);
            
            // Leave one army on source
            selectedTile.armyCount = 1;
            selectedTile.UpdateTileAppearance();
        }
        else
        {
            // Defender wins: target keeps its color with surviving armies
            target.armyCount = result.defenderSurvivors;
            target.UpdateTileAppearance();
            
            // Source loses the attacking armies, keeps 1 + any survivors
            selectedTile.armyCount = 1 + result.attackerSurvivors;
            selectedTile.UpdateTileAppearance();
        }
        
        // Update visuals and UI
        GameManager.Instance.CallUpdateAllArmyVisuals();
        GameManager.Instance.CallUpdateUI();
        
        // Check win condition after attack
        GameManager.Instance.CheckWinCondition();
        if (GameManager.Instance.gameEnded)
        {
            return;
        }
        
        // Determine which tile to select next (if any)
        HexTile nextSelectedTile = null;
        if (result.attackerWins && target.armyCount >= 2)
        {
            // Attacker won and can continue attacking from new tile
            nextSelectedTile = target;
        }
        else if (!result.attackerWins && selectedTile.armyCount >= 2)
        {
            // Attacker lost but still has armies to attack with
            nextSelectedTile = selectedTile;
        }
        
        if (nextSelectedTile != null)
        {
            // Select the tile that can continue attacking
            selectedTile.RestoreColor();
            selectedTile = nextSelectedTile;
            selectedTile.Highlight();
            ClearHighlights();
            HighlightAttackableNeighbors(selectedTile);
            UpdateInfoPanel(selectedTile);
        }
        else
        {
            // No more attacks possible, deselect
            selectedTile.RestoreColor();
            selectedTile = null;
            ClearHighlights();
            UpdateInfoPanel(target);
        }
    }

    private void ShowBattleResultPopup(string message)
    {
        if (battleResultPopup == null)
        {
            Canvas canvas = FindFirstObjectByType<Canvas>();
            battleResultPopup = new GameObject("BattleResultPopup");
            battleResultPopup.transform.SetParent(canvas.transform, false);
            Image bg = battleResultPopup.AddComponent<Image>();
            bg.color = new Color(0, 0, 0, 0.7f);
            RectTransform rect = battleResultPopup.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0);
            rect.anchorMax = new Vector2(0.5f, 0);
            rect.pivot = new Vector2(0.5f, 0);
            rect.anchoredPosition = new Vector2(0, 60);
            rect.sizeDelta = new Vector2(420, 140); // Increased height for 5 lines
            GameObject textObj = new GameObject("BattleResultText");
            textObj.transform.SetParent(battleResultPopup.transform, false);
            battleResultText = textObj.AddComponent<TextMeshProUGUI>();
            battleResultText.fontSize = 22;
            battleResultText.color = Color.white;
            battleResultText.alignment = TextAlignmentOptions.Center;
            RectTransform textRect = battleResultText.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
        }
        battleResultPopup.SetActive(true);
        battleResultText.text = message;
        battleResultText.alpha = 1f;
        Image popupBg = battleResultPopup.GetComponent<Image>();
        popupBg.color = new Color(0, 0, 0, 0.7f);
        if (battleResultCoroutine != null)
        {
            StopCoroutine(battleResultCoroutine);
        }
        battleResultCoroutine = StartCoroutine(FadeBattleResultPopup());
    }

    private IEnumerator FadeBattleResultPopup()
    {
        yield return new WaitForSeconds(2f);
        float fadeTime = 1f;
        float t = 0f;
        Image popupBg = battleResultPopup.GetComponent<Image>();
        Color bgColor = popupBg.color;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / fadeTime);
            battleResultText.alpha = alpha;
            popupBg.color = new Color(bgColor.r, bgColor.g, bgColor.b, 0.7f * alpha);
            yield return null;
        }
        battleResultPopup.SetActive(false);
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
