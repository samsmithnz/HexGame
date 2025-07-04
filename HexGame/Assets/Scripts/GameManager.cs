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

    void Awake()
    {
        if (Instance == null) Instance = this;
        turn = 1;
        currentPlayer = HexColor.Blue;
        CreateGameUI();
        UpdateUI();
    }

    public void AddArmies(HexColor player, int amount)
    {
        if (player == HexColor.Blue) blueArmies += amount;
        else if (player == HexColor.Green) greenArmies += amount;
        UpdateUI();
    }

    public void SetArmies(HexColor player, int amount)
    {
        if (player == HexColor.Blue) blueArmies = amount;
        else if (player == HexColor.Green) greenArmies = amount;
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
        UpdateUI();
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
        }
        else
        {
            canvas = canvasObj.GetComponent<Canvas>();
        }
        // Panel
        uiPanel = new GameObject("TurnPanel");
        uiPanel.transform.SetParent(canvas.transform, false);
        Image panelImage = uiPanel.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.7f);
        RectTransform panelRect = uiPanel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(1, 1); // Top right
        panelRect.anchorMax = new Vector2(1, 1); // Top right
        panelRect.pivot = new Vector2(1, 1);     // Top right
        panelRect.anchoredPosition = new Vector2(-10, -10); // 10px from top right
        panelRect.sizeDelta = new Vector2(260, 120); // Increased height for spacing
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
        armiesRect.sizeDelta = new Vector2(-20, 30);
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
        // Button Text
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
    }

    private void UpdateUI()
    {
        string playerStr = currentPlayer == HexColor.Blue ? "Blue" : "Green";
        turnText.text = $"Turn {turn}: {playerStr}";
        armiesText.text = $"Blue Armies: {blueArmies}\nGreen Armies: {greenArmies}";
    }
}
