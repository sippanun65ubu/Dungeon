using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class InventoryItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string itemName;
    public string itemDescription;
    public string itemStats; // E.g., "Health: +10, Stamina: +5"
    public Vector3 customTooltipOffset = new Vector3(10f, -10f, 0f); // Offset for tooltip position

    private Coroutine tooltipCoroutine;
    private bool isPointerOver = false; // Tracks whether the cursor is still over the item

    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerOver = true;

        // Stop any running tooltip coroutine to prevent multiple executions
        if (tooltipCoroutine != null)
        {
            StopCoroutine(tooltipCoroutine);
        }

        // Start the coroutine to show the tooltip after a delay
        tooltipCoroutine = StartCoroutine(ShowTooltipWithDelay());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerOver = false;

        // Stop the tooltip coroutine
        if (tooltipCoroutine != null)
        {
            StopCoroutine(tooltipCoroutine);
            tooltipCoroutine = null;
        }

        // Hide the tooltip immediately
        TooltipUI.Instance.HideTooltip();
    }

    private IEnumerator ShowTooltipWithDelay()
    {
        yield return new WaitForSeconds(0.5f); // Delay before showing tooltip

        // Check if the cursor is still over the item
        if (isPointerOver)
        {
            Vector3 tooltipPosition = Input.mousePosition + customTooltipOffset;
            TooltipUI.Instance.ShowTooltip(itemName, itemDescription, itemStats, tooltipPosition);
        }
    }
}
