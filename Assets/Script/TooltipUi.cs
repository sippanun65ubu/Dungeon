using UnityEngine;
using UnityEngine.UI;

public class TooltipUI : MonoBehaviour
{
    public static TooltipUI Instance;

    public GameObject tooltipPanel;      // Tooltip UI Panel
    public Text itemNameText;           // Text for item name
    public Text itemDescriptionText;    // Text for item description
    public Text itemStatsText;          // Text for item stats

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        tooltipPanel.SetActive(false); // Hide the tooltip at the start
    }

    /// <summary>
    /// Shows the tooltip with the provided item details.
    /// </summary>
    /// <param name="name">Item name.</param>
    /// <param name="description">Item description.</param>
    /// <param name="stats">Item stats.</param>
    /// <param name="position">Optional position for the tooltip. Defaults to mouse position.</param>
    public void ShowTooltip(string name, string description, string stats, Vector3? position = null)
    {
        tooltipPanel.SetActive(true); // Enable the tooltip panel

        // Set the tooltip text
        itemNameText.text = name;
        itemDescriptionText.text = description;
        itemStatsText.text = stats;

        // Set the tooltip position, clamped to screen bounds
        Vector3 rawPosition = position ?? Input.mousePosition + new Vector3(10f, -10f, 0f);
        tooltipPanel.transform.position = ClampToScreenBounds(rawPosition);
    }

    /// <summary>
    /// Hides the tooltip.
    /// </summary>
    public void HideTooltip()
    {
        tooltipPanel.SetActive(false); // Hide the tooltip
    }

    /// <summary>
    /// Ensures the tooltip remains within the screen bounds.
    /// </summary>
    private Vector3 ClampToScreenBounds(Vector3 position)
    {
        RectTransform tooltipRect = tooltipPanel.GetComponent<RectTransform>();
        float tooltipWidth = tooltipRect.rect.width;
        float tooltipHeight = tooltipRect.rect.height;

        // Clamp position to screen bounds
        position.x = Mathf.Clamp(position.x, 0, Screen.width - tooltipWidth);
        position.y = Mathf.Clamp(position.y, tooltipHeight, Screen.height);

        return position;
    }
}
