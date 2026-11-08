using UnityEngine;
using UnityEngine.UI;

public class HelpPopupManager : MonoBehaviour
{
    private GameObject helpPopup;

    public void CreateHelpPopup(Canvas canvas)
    {
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
        popupBgButton.onClick.AddListener(HideHelpPopup); // Close when clicking background

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

        // --- Scrollable Help Text ---
        GameObject scrollObj = new GameObject("HelpScrollRect");
        scrollObj.transform.SetParent(contentPanel.transform, false);
        ScrollRect scrollRect = scrollObj.AddComponent<ScrollRect>();
        Image scrollBg = scrollObj.AddComponent<Image>();
        scrollBg.color = new Color(0, 0, 0, 0.0f); // Transparent background
        RectTransform scrollRectTransform = scrollObj.GetComponent<RectTransform>();
        scrollRectTransform.anchorMin = new Vector2(0, 0.08f);
        scrollRectTransform.anchorMax = new Vector2(1, 0.88f);
        scrollRectTransform.offsetMin = new Vector2(10, 10);
        scrollRectTransform.offsetMax = new Vector2(-10, -10);
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.movementType = ScrollRect.MovementType.Clamped;

        // Viewport for scrolling
        GameObject viewportObj = new GameObject("Viewport");
        viewportObj.transform.SetParent(scrollObj.transform, false);
        RectTransform viewportRect = viewportObj.AddComponent<RectTransform>();
        viewportRect.anchorMin = Vector2.zero;
        viewportRect.anchorMax = Vector2.one;
        viewportRect.offsetMin = Vector2.zero;
        viewportRect.offsetMax = new Vector2(-20, 0); // Leave space for scrollbar
        Image viewportImage = viewportObj.AddComponent<Image>();
        viewportImage.color = new Color(0, 0, 0, 0.0f);
        Mask viewportMask = viewportObj.AddComponent<Mask>();
        viewportMask.showMaskGraphic = false;
        scrollRect.viewport = viewportRect;

        // Content container for scrollable content
        GameObject contentObj = new GameObject("Content");
        contentObj.transform.SetParent(viewportObj.transform, false);
        RectTransform contentRectTransform = contentObj.AddComponent<RectTransform>();
        contentRectTransform.anchorMin = new Vector2(0, 1);
        contentRectTransform.anchorMax = new Vector2(1, 1);
        contentRectTransform.pivot = new Vector2(0.5f, 1);
        contentRectTransform.anchoredPosition = Vector2.zero;
        contentRectTransform.sizeDelta = new Vector2(0, 800); // Fixed height for scrolling content
        scrollRect.content = contentRectTransform;

        // Help Text inside the scrollable content
        GameObject helpTextObj = new GameObject("HelpText");
        helpTextObj.transform.SetParent(contentObj.transform, false);
        Text helpText = helpTextObj.AddComponent<Text>();
        helpText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        helpText.fontSize = 14;
        helpText.color = Color.white;
        helpText.alignment = TextAnchor.UpperLeft;
        helpText.text = "HEX GAME RULES:\n\n" +
                       "OBJECTIVE & WINNING:\n" +
                       "• Control territories and build armies.\n" +
                       "• A player wins when they conquer all tiles.\n" +
                       "• A popup will appear showing the winner and a 'Play again?' button.\n\n" +
                       "GAMEPLAY:\n" +
                       "• Two players: Blue and Green.\n" +
                       "• Blue player always goes first.\n" +
                       "• Turn counter increases when Blue's turn begins.\n\n" +
                       "STARTING SETUP:\n" +
                       "• Each player starts with 3 adjacent tiles, each with 3 armies.\n" +
                       "• All other tiles are blank.\n\n" +
                       "ARMIES:\n" +
                       "• At the start of each turn, gain 1 army per tile you control (max 7 per tile).\n" +
                       "• Armies are represented by colored cylinders on tiles.\n\n" +
                       "ATTACKING:\n" +
                       "• Click your tile with 2 or more armies to select it.\n" +
                       "• Orange highlight shows tiles you can attack.\n" +
                       "• Combat uses Risk-style dice rolling: each army rolls one die.\n" +
                       "• Higher rolls win. Defender wins ties.\n" +
                       "• Winner takes the tile. If attacker wins, surviving armies occupy the tile.\n" +
                       "• If your tile has 2 or more armies after combat, it stays selected for more attacks.\n\n" +
                       "CONTROLS:\n" +
                       "• Click on tiles to select and view information.\n" +
                       "• Use 'Next Turn' button to end your turn.\n" +
                       "• Press ESC or click Close to exit this help.\n" +
                       "• WASD keys to move camera around the map.\n\n" +
                       "TILE INFORMATION:\n" +
                       "• Selected tile info appears in top-left panel.\n" +
                       "• Shows tile coordinates, color, and army count.\n\n" +
                       "VISUAL INDICATORS:\n" +
                       "• Blue tiles belong to Blue player.\n" +
                       "• Green tiles belong to Green player.\n" +
                       "• Gray tiles are neutral (none).\n" +
                       "• Yellow highlight shows selected tile.\n" +
                       "• Orange highlight shows attackable tiles.\n" +
                       "• Small cylinders represent armies on tiles.\n\n" +
                       "GAME STATUS:\n" +
                       "• Current turn and active player shown in top-right.\n" +
                       "• Total army count displayed for each player.\n" +
                       "• Turn advances when Next Turn button is clicked.";
        RectTransform helpTextRect = helpText.GetComponent<RectTransform>();
        helpTextRect.anchorMin = Vector2.zero;
        helpTextRect.anchorMax = Vector2.one;
        helpTextRect.offsetMin = new Vector2(10, 10);
        helpTextRect.offsetMax = new Vector2(-10, -10);
        helpText.horizontalOverflow = HorizontalWrapMode.Wrap;
        helpText.verticalOverflow = VerticalWrapMode.Overflow;

        // Scrollbar
        GameObject scrollbarObj = new GameObject("HelpScrollbar");
        scrollbarObj.transform.SetParent(scrollObj.transform, false);
        Scrollbar scrollbar = scrollbarObj.AddComponent<Scrollbar>();
        Image scrollbarImage = scrollbarObj.AddComponent<Image>();
        scrollbarImage.color = new Color(0.7f, 0.7f, 0.7f, 1f);
        RectTransform scrollbarRect = scrollbarObj.GetComponent<RectTransform>();
        scrollbarRect.anchorMin = new Vector2(1, 0);
        scrollbarRect.anchorMax = new Vector2(1, 1);
        scrollbarRect.pivot = new Vector2(1, 0.5f);
        scrollbarRect.sizeDelta = new Vector2(20, 0);
        scrollbarRect.offsetMin = new Vector2(-20, 0);
        scrollbarRect.offsetMax = new Vector2(0, 0);
        scrollbar.direction = Scrollbar.Direction.BottomToTop;
        scrollbar.targetGraphic = scrollbarImage;
        scrollRect.verticalScrollbar = scrollbar;
        scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;

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
        closeButton.onClick.AddListener(HideHelpPopup);

        // Start with help popup hidden
        helpPopup.SetActive(false);
    }

    public void ShowHelpPopup()
    {
        if (helpPopup != null)
        {
            helpPopup.SetActive(true);
        }
    }

    public void HideHelpPopup()
    {
        if (helpPopup != null)
        {
            helpPopup.SetActive(false);
        }
    }
}
