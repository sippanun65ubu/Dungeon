using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveInGame : MonoBehaviour
{
    public Button button;
    public TextMeshProUGUI buttonText;

    [Header("Overwrite Confirmation")]
    public OverwriteConfirmation confirmationDialog;

    private const string SlotUsedKey = "SaveSlotUsed";

    private void Awake()
    {
        // Grab references (assuming your hierarchy has a TMP child named "Text (TMP)")
        button = GetComponent<Button>();
        buttonText = transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        // Set initial button text
        RefreshButtonText();

        // Hook up click
        button.onClick.AddListener(OnButtonClick);
    }

    private void RefreshButtonText()
    {
        bool used = PlayerPrefs.HasKey(SlotUsedKey);
        buttonText.text = used
            ? "Save Game (Saved)"
            : "Save Game";
    }

    private void OnButtonClick()
    {
        bool used = PlayerPrefs.HasKey(SlotUsedKey);

        if (used)
        {
            // Already have a save—ask before overwriting
            if (confirmationDialog != null)
            {
                confirmationDialog.Show(
                    "You already have a save. Overwrite?",
                    () => { OverwriteSlot(); DeselectButton(); },
                    () => { DeselectButton(); }
                );
            }
            else
            {
                Debug.LogWarning("ConfirmationDialog not assigned!");
                OverwriteSlot();
            }
        }
        else
        {
            // First‐time save
            OverwriteSlot();
            DeselectButton();
        }
    }

    private void OverwriteSlot()
    {
        // Save via PlayFab manager
        if (PLayFabManager.Instance != null)
        {
            PLayFabManager.Instance.SaveGameData();
        }
        else
        {
            Debug.LogError("PLayFabManager.Instance is null!");
        }

        // Mark this slot as used
        PlayerPrefs.SetInt(SlotUsedKey, 1);
        PlayerPrefs.Save();

        // Update the button label
        RefreshButtonText();
        Debug.Log("Game saved.");
    }

    private void DeselectButton()
    {
        var es = GameObject.FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
        if (es != null)
            es.SetSelectedGameObject(null);
    }
}
