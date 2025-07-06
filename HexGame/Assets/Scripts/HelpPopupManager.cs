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

        // Help Content Text (no scroll view)
        GameObject helpTextObj = new GameObject("HelpText");
        helpTextObj.transform.SetParent(contentPanel.transform, false);
        Text helpText = helpTextObj.AddComponent<Text>();
        helpText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        helpText.fontSize = 16;
        helpText.color = Color.white;
        helpText.alignment = TextAnchor.UpperLeft;
        helpText.text = "HEX GAME RULES:\n\n" +
                       "OBJECTIVE & WINNING:\n" +
                       "• Control territories and build armies.\n" +
                       "• If a player has no tiles or no armies, the other player wins.\n" +
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
                       "• When you attack, all but one army move to the new tile.\n" +
                       "• If your new tile has 2 or more armies, it stays selected and you may attack again.\n\n" +
                       "CONTROLS:\n" +
                       "• Click on tiles to select and view information.\n" +
                       "• Use 'Next Turn' button to end your turn.\n" +
                       "• Press ESC or click Close to exit this help.\n\n" +
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
