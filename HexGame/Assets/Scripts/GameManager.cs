using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public HexColor currentPlayer = HexColor.Blue;
    public int blueArmies = 0;
    public int greenArmies = 0;
    public int turn = 1;

    private GameObject uiPanel;
    private Text turnText;
    private Text armiesText;
    private Button nextTurnButton;
    private Button helpButton;
    private GameObject helpPopup;

    private List<HexTile> allTiles = new List<HexTile>();
    private GameObject armyPrefab;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        turn = 1;
        currentPlayer = HexColor.Blue;
        CreateGameUI();
        FindAllTiles();
        CreateArmyPrefab();
        AwardInitialArmies();
        UpdateAllArmyVisuals();
        UpdateUI();
    }

    void Update()
    {
        // Close help popup with ESC key
        if (Input.GetKeyDown(KeyCode.Escape) && helpPopup != null && helpPopup.activeInHierarchy)
        {
            CloseHelpPopup();
        }
    }

    private void FindAllTiles()
    {
        allTiles.Clear();
        allTiles.AddRange(FindObjectsByType<HexTile>(FindObjectsSortMode.None));
    }

    private void CreateArmyPrefab()
    {
        // Create a small cylinder prefab for armies
        armyPrefab = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        armyPrefab.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        armyPrefab.GetComponent<Collider>().enabled = false;
        // Set prefab to active so instances are active by default
        armyPrefab.SetActive(true);
    }

    private void AwardInitialArmies()
    {
        blueArmies = 0;
        greenArmies = 0;
        foreach (HexTile tile in allTiles)
        {
            tile.armyCount = 1;
            if (tile.hexColor == HexColor.Blue)
            {
                blueArmies++;
            }
            else if (tile.hexColor == HexColor.Green)
            {
                greenArmies++;
            }
        }
        UpdateAllArmyVisuals(); // Ensure visuals are created after initial armies
    }

    public void AddArmies(HexColor player, int amount)
    {
        if (player == HexColor.Blue)
        {
            blueArmies += amount;
        }
        else if (player == HexColor.Green)
        {
            greenArmies += amount;
        }
        UpdateUI();
    }

    public void SetArmies(HexColor player, int amount)
    {
        if (player == HexColor.Blue)
        {
            blueArmies = amount;
        }
        else if (player == HexColor.Green)
        {
            greenArmies = amount;
        }
        UpdateUI();
    }

    public void NextTurn()
    {
        if (currentPlayer == HexColor.Green)
        {
            currentPlayer = HexColor.Blue;
            turn++;
        }
        else
        {
            currentPlayer = HexColor.Green;
        }
        AwardArmiesToCurrentPlayer();
        UpdateAllArmyVisuals(); // Ensure visuals are updated after awarding armies
        UpdateUI();
    }

    private void AwardArmiesToCurrentPlayer()
    {
        int armiesAwarded = 0;
        foreach (HexTile tile in allTiles)
        {
            if (tile.hexColor == currentPlayer)
            {
                if (tile.armyCount < 7)
                {
                    tile.armyCount++;
                    armiesAwarded++;
                }
            }
        }
        if (currentPlayer == HexColor.Blue)
        {
            blueArmies += armiesAwarded;
        }
        else if (currentPlayer == HexColor.Green)
        {
            greenArmies += armiesAwarded;
        }
        UpdateAllArmyVisuals(); // Ensure visuals are updated after awarding armies
    }

    private void UpdateAllArmyVisuals()
    {
        foreach (HexTile tile in allTiles)
        {
            PlaceArmyVisual(tile);
        }
    }

    private void PlaceArmyVisual(HexTile tile)
    {
        // Remove old army visuals
        foreach (Transform child in tile.transform)
        {
            if (child.name.StartsWith("ArmyVisual"))
            {
                Destroy(child.gameObject);
            }
        }
        // Get hex height (default to 0.2 if not found)
        float hexHeight = 0.2f;
        HexagonMesh mesh = tile.GetComponent<HexagonMesh>();
        if (mesh != null)
        {
            // Try to get the private hexRadius via reflection (since it's private)
            System.Reflection.FieldInfo radiusField = typeof(HexagonMesh).GetField("hexRadius", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (radiusField != null)
            {
                float radius = (float)radiusField.GetValue(mesh);
                hexHeight = radius * 0.2f;
            }
        }
        float armyHeight = 0.1f; // cylinder Y scale
        float y = hexHeight + (armyHeight / 2f);
        int n = Mathf.Min(tile.armyCount, 7); // Max 7 armies visualized
        for (int i = 0; i < n; i++)
        {
            Vector3 pos;
            if (i == 0)
            {
                // Center
                pos = new Vector3(0, y, 0);
            }
            else
            {
                // 6 around the center
                float angle = 2 * Mathf.PI * (i - 1) / 6f;
                float radiusCircle = 0.25f;
                float x = Mathf.Cos(angle) * radiusCircle;
                float z = Mathf.Sin(angle) * radiusCircle;
                pos = new Vector3(x, y, z);
            }
            GameObject army = Instantiate(armyPrefab, tile.transform);
            army.name = "ArmyVisual" + i;
            army.transform.localPosition = pos;
            army.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            // Assign a unique material instance for color
            Renderer renderer = army.GetComponent<Renderer>();
            renderer.material = new Material(renderer.material);
            if (tile.hexColor == HexColor.Blue)
            {
                renderer.material.color = Color.blue;
            }
            else if (tile.hexColor == HexColor.Green)
            {
                renderer.material.color = Color.green;
            }
        }
    }

    public void ShowHelpPopup()
    {
        if (helpPopup != null)
        {
            helpPopup.SetActive(true);
        }
    }

    public void CloseHelpPopup()
    {
        if (helpPopup != null)
        {
            helpPopup.SetActive(false);
        }
    }

    private void CreateGameUI()
    {
        // Always create a dedicated canvas for the GameManager UI if it doesn't exist
        GameObject canvasObj = GameObject.Find("GameCanvas");
        Canvas canvas;
        if (canvasObj == null)
        {
            canvasObj = new GameObject("GameCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            // Add EventSystem if not present
            if (FindObjectsByType<UnityEngine.EventSystems.EventSystem>(FindObjectsSortMode.None).Length == 0)
            {
                GameObject es = new GameObject("EventSystem");
                es.AddComponent<UnityEngine.EventSystems.EventSystem>();
                es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }
        }
        else
        {
            canvas = canvasObj.GetComponent<Canvas>();
        }
        
        // Main Game Panel (Top Right)
        uiPanel = new GameObject("TurnPanel");
        uiPanel.transform.SetParent(canvas.transform, false);
        Image panelImage = uiPanel.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.7f);
        RectTransform panelRect = uiPanel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(1, 1); // Top right
        panelRect.anchorMax = new Vector2(1, 1); // Top right
        panelRect.pivot = new Vector2(1, 1);     // Top right
        panelRect.anchoredPosition = new Vector2(-10, -10); // 10px from top right
        panelRect.sizeDelta = new Vector2(260, 160); // Increased height for more vertical space
        
        // Turn Text
        GameObject turnObj = new GameObject("TurnText");
        turnObj.transform.SetParent(uiPanel.transform, false);
        turnText = turnObj.AddComponent<Text>();
        turnText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        turnText.fontSize = 18;
        turnText.color = Color.white;
        turnText.alignment = TextAnchor.UpperLeft;
        RectTransform turnRect = turnText.GetComponent<RectTransform>();
        turnRect.anchorMin = new Vector2(0, 1);
        turnRect.anchorMax = new Vector2(1, 1);
        turnRect.pivot = new Vector2(0, 1);
        turnRect.anchoredPosition = new Vector2(10, -10);
        turnRect.sizeDelta = new Vector2(-20, 30);
        
        // Armies Text
        GameObject armiesObj = new GameObject("ArmiesText");
        armiesObj.transform.SetParent(uiPanel.transform, false);
        armiesText = armiesObj.AddComponent<Text>();
        armiesText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        armiesText.fontSize = 16;
        armiesText.color = Color.white;
        armiesText.alignment = TextAnchor.UpperLeft;
        RectTransform armiesRect = armiesText.GetComponent<RectTransform>();
        armiesRect.anchorMin = new Vector2(0, 1);
        armiesRect.anchorMax = new Vector2(1, 1);
        armiesRect.pivot = new Vector2(0, 1);
        armiesRect.anchoredPosition = new Vector2(10, -45); // Move down for more space
        armiesRect.sizeDelta = new Vector2(-20, 60); // Increased height for more lines
        
        // Next Turn Button
        GameObject buttonObj = new GameObject("NextTurnButton");
        buttonObj.transform.SetParent(uiPanel.transform, false);
        nextTurnButton = buttonObj.AddComponent<Button>();
        Image btnImage = buttonObj.AddComponent<Image>();
        btnImage.color = new Color(0.2f, 0.2f, 0.8f, 1f);
        RectTransform btnRect = buttonObj.GetComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0, 0);
        btnRect.anchorMax = new Vector2(1, 0);
        btnRect.pivot = new Vector2(0.5f, 0);
        btnRect.anchoredPosition = new Vector2(0, 10);
        btnRect.sizeDelta = new Vector2(-20, 30);
        
        // Next Turn Button Text
        GameObject btnTextObj = new GameObject("ButtonText");
        btnTextObj.transform.SetParent(buttonObj.transform, false);
        Text btnText = btnTextObj.AddComponent<Text>();
        btnText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        btnText.fontSize = 16;
        btnText.color = Color.white;
        btnText.alignment = TextAnchor.MiddleCenter;
        btnText.text = "Next Turn";
        RectTransform btnTextRect = btnText.GetComponent<RectTransform>();
        btnTextRect.anchorMin = Vector2.zero;
        btnTextRect.anchorMax = Vector2.one;
        btnTextRect.offsetMin = Vector2.zero;
        btnTextRect.offsetMax = Vector2.zero;
        nextTurnButton.onClick.AddListener(NextTurn);

        // Help Button (Bottom Left)
        GameObject helpButtonObj = new GameObject("HelpButton");
        helpButtonObj.transform.SetParent(canvas.transform, false);
        helpButton = helpButtonObj.AddComponent<Button>();
        Image helpBtnImage = helpButtonObj.AddComponent<Image>();
        helpBtnImage.color = new Color(0.2f, 0.6f, 1f, 1f); // Light blue color
        RectTransform helpBtnRect = helpButtonObj.GetComponent<RectTransform>();
        helpBtnRect.anchorMin = new Vector2(0, 0); // Bottom left
        helpBtnRect.anchorMax = new Vector2(0, 0); // Bottom left
        helpBtnRect.pivot = new Vector2(0, 0);     // Bottom left
        helpBtnRect.anchoredPosition = new Vector2(10, 10); // 10px from bottom left
        helpBtnRect.sizeDelta = new Vector2(60, 30);
        
        // Help Button Text
        GameObject helpBtnTextObj = new GameObject("HelpButtonText");
        helpBtnTextObj.transform.SetParent(helpButtonObj.transform, false);
        Text helpBtnText = helpBtnTextObj.AddComponent<Text>();
        helpBtnText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        helpBtnText.fontSize = 14;
        helpBtnText.color = Color.white;
        helpBtnText.alignment = TextAnchor.MiddleCenter;
        helpBtnText.text = "Help";
        RectTransform helpBtnTextRect = helpBtnText.GetComponent<RectTransform>();
        helpBtnTextRect.anchorMin = Vector2.zero;
        helpBtnTextRect.anchorMax = Vector2.one;
        helpBtnTextRect.offsetMin = Vector2.zero;
        helpBtnTextRect.offsetMax = Vector2.zero;
        helpButton.onClick.AddListener(ShowHelpPopup);

        // Help Popup - Make it clickable background overlay but not full screen content
        helpPopup = new GameObject("HelpPopup");
        helpPopup.transform.SetParent(canvas.transform, false);
        Button popupBgButton = helpPopup.AddComponent<Button>(); // Make background clickable to close
        Image popupBg = helpPopup.AddComponent<Image>();
        popupBg.color = new Color(0, 0, 0, 0.5f); // Semi-transparent overlay
        RectTransform popupRect = helpPopup.GetComponent<RectTransform>();
        popupRect.anchorMin = Vector2.zero;
        popupRect.anchorMax = Vector2.one;
        popupRect.offsetMin = Vector2.zero;
        popupRect.offsetMax = Vector2.zero;
        popupBgButton.onClick.AddListener(CloseHelpPopup); // Close when clicking background

        // Help Content Panel - Centered and reasonably sized
        GameObject contentPanel = new GameObject("HelpContentPanel");
        contentPanel.transform.SetParent(helpPopup.transform, false);
        Image contentBg = contentPanel.AddComponent<Image>();
        contentBg.color = new Color(0.1f, 0.1f, 0.1f, 0.95f);
        RectTransform contentRect = contentPanel.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0.15f, 0.15f); // Smaller centered window
        contentRect.anchorMax = new Vector2(0.85f, 0.85f); // Smaller centered window
        contentRect.offsetMin = Vector2.zero;
        contentRect.offsetMax = Vector2.zero;

        // Help Title
        GameObject titleObj = new GameObject("HelpTitle");
        titleObj.transform.SetParent(contentPanel.transform, false);
        Text titleText = titleObj.AddComponent<Text>();
        titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        titleText.fontSize = 24;
        titleText.color = Color.white;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.text = "Game Rules";
        RectTransform titleRect = titleText.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0.9f);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.offsetMin = new Vector2(10, -10);
        titleRect.offsetMax = new Vector2(-10, -10);

        // Help Content Text (no scroll view)
        GameObject helpTextObj = new GameObject("HelpText");
        helpTextObj.transform.SetParent(contentPanel.transform, false);
        Text helpText = helpTextObj.AddComponent<Text>();
        helpText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        helpText.fontSize = 16;
        helpText.color = Color.white;
        helpText.alignment = TextAnchor.UpperLeft;
        helpText.text = "HEX GAME RULES:\n\n" +
                       "OBJECTIVE:\n" +
                       "• Control territories and build armies\n\n" +
                       "GAMEPLAY:\n" +
                       "• Two players: Blue and Green\n" +
                       "• Blue player always goes first\n" +
                       "• Turn counter increases when Blue's turn begins\n\n" +
                       "ARMIES:\n" +
                       "• Each player starts with 1 army on each tile they control\n" +
                       "• At the start of each turn, gain 1 army per tile you control\n" +
                       "• Armies are represented by colored cylinders on tiles\n\n" +
                       "CONTROLS:\n" +
                       "• Click on tiles to select and view information\n" +
                       "• Use 'Next Turn' button to end your turn\n" +
                       "• Press ESC or click Close to exit this help\n\n" +
                       "CAMERA:\n" +
                       "• WASD keys to move camera around the map\n\n" +
                       "TILE INFORMATION:\n" +
                       "• Selected tile info appears in top-left panel\n" +
                       "• Shows tile coordinates, color, and army count\n\n" +
                       "VISUAL INDICATORS:\n" +
                       "• Blue tiles belong to Blue player\n" +
                       "• Green tiles belong to Green player\n" +
                       "• Gray tiles are neutral (none)\n" +
                       "• Yellow highlight shows selected tile\n" +
                       "• Small cylinders represent armies on tiles\n\n" +
                       "GAME STATUS:\n" +
                       "• Current turn and active player shown in top-right\n" +
                       "• Total army count displayed for each player\n" +
                       "• Turn advances when Next Turn button is clicked";
        RectTransform helpTextRect = helpText.GetComponent<RectTransform>();
        helpTextRect.anchorMin = new Vector2(0, 0);
        helpTextRect.anchorMax = new Vector2(1, 0.85f);
        helpTextRect.offsetMin = new Vector2(20, 10);
        helpTextRect.offsetMax = new Vector2(-20, -10);

        // Close Button
        GameObject closeButtonObj = new GameObject("CloseButton");
        closeButtonObj.transform.SetParent(contentPanel.transform, false);
        Button closeButton = closeButtonObj.AddComponent<Button>();
        Image closeBtnImage = closeButtonObj.AddComponent<Image>();
        closeBtnImage.color = new Color(0.8f, 0.2f, 0.2f, 1f);
        RectTransform closeBtnRect = closeButtonObj.GetComponent<RectTransform>();
        closeBtnRect.anchorMin = new Vector2(0.4f, 0.02f);
        closeBtnRect.anchorMax = new Vector2(0.6f, 0.08f);
        closeBtnRect.offsetMin = Vector2.zero;
        closeBtnRect.offsetMax = Vector2.zero;
        
        // Close Button Text
        GameObject closeBtnTextObj = new GameObject("CloseButtonText");
        closeBtnTextObj.transform.SetParent(closeButtonObj.transform, false);
        Text closeBtnText = closeBtnTextObj.AddComponent<Text>();
        closeBtnText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        closeBtnText.fontSize = 16;
        closeBtnText.color = Color.white;
        closeBtnText.alignment = TextAnchor.MiddleCenter;
        closeBtnText.text = "Close";
        RectTransform closeBtnTextRect = closeBtnText.GetComponent<RectTransform>();
        closeBtnTextRect.anchorMin = Vector2.zero;
        closeBtnTextRect.anchorMax = Vector2.one;
        closeBtnTextRect.offsetMin = Vector2.zero;
        closeBtnTextRect.offsetMax = Vector2.zero;
        closeButton.onClick.AddListener(CloseHelpPopup);

        // Start with help popup hidden
        helpPopup.SetActive(false);
    }

    private void UpdateUI()
    {
        // Count tiles and armies for each player
        int blueTiles = 0;
        int greenTiles = 0;
        int blueTotalArmies = 0;
        int greenTotalArmies = 0;
        int noneTiles = 0;
        foreach (HexTile tile in allTiles)
        {
            if (tile.hexColor == HexColor.Blue)
            {
                blueTiles++;
                blueTotalArmies += tile.armyCount;
            }
            else if (tile.hexColor == HexColor.Green)
            {
                greenTiles++;
                greenTotalArmies += tile.armyCount;
            }
            else
            {
                noneTiles++;
            }
        }
        Debug.Log($"Tiles: Blue={blueTiles}, Green={greenTiles}, None={noneTiles}");
        string playerStr = currentPlayer == HexColor.Blue ? "Blue" : "Green";
        turnText.text = $"Turn {turn}: {playerStr}";
        armiesText.text =
            $"Blue Tiles: {blueTiles}  |  Green Tiles: {greenTiles}\n" +
            $"Blue Armies: {blueTotalArmies}  |  Green Armies: {greenTotalArmies}";
    }
}
