using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
    public Button button;
    public TextMeshProUGUI buttonText;
    public int slotNumber;

    // Reference to your confirmation dialog (assign via Inspector).
    public OverwriteConfirmation confirmationDialog;

    // We’ll store both a key to mark the slot as used and a key for the text.
    private string slotUsedKey;
    private string slotTextKey;

    private void Awake()
    {
        // Get references
        button = GetComponent<Button>();
        buttonText = transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();

        // Build keys for PlayerPrefs based on slotNumber
        slotUsedKey = "SaveSlot_" + slotNumber;
        slotTextKey = "SaveSlotText_" + slotNumber;
    }

    private void Start()
    {
        // Restore text from PlayerPrefs if it exists
        if (PlayerPrefs.HasKey(slotTextKey))
        {
            string loadedText = PlayerPrefs.GetString(slotTextKey);
            buttonText.text = loadedText;
            Debug.Log($"Slot {slotNumber} text loaded: {loadedText}");
        }
        else
        {
            Debug.Log($"Slot {slotNumber} has no saved text in PlayerPrefs.");
        }

        // Add button click listener
        button.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        DateTime dt = DateTime.Now;
        string time = dt.ToString("yyyy-MM-dd HH-mm");
        int score = GameManager.instance.totalScore;

        // Create the new text for this slot
        string newText = $"Save Game {slotNumber} | {time} | Score: {score}";

        if (!IsSlotEmpty())
        {
            // Slot is not empty. Ask the player for confirmation.
            if (confirmationDialog != null)
            {
                confirmationDialog.Show("This save slot is already used. Overwrite?",
                    () => { OverwriteSlot(newText); DeselectButton(); },
                    () => { DeselectButton(); });
            }
            else
            {
                Debug.LogError("Confirmation dialog is not assigned!");
            }
        }
        else
        {
            // Slot is empty; save directly.
            OverwriteSlot(newText);
            DeselectButton();
        }
    }

    // Actually overwrites the slot text and calls SaveGame
    private void OverwriteSlot(string newText)
    {
        // Update the button text
        buttonText.text = newText;

        // Save the game
        SaveGame();

        // Save the new text in PlayerPrefs
        PlayerPrefs.SetString(slotTextKey, newText);
        PlayerPrefs.Save();

        Debug.Log($"Slot {slotNumber} overwritten with text: {newText}");
    }

    private void DeselectButton()
    {
        GameObject eventSystem = GameObject.Find("EventSystem");
        if (eventSystem != null)
        {
            eventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
        }
    }

    // Check if the save slot is empty via a PlayerPrefs key.
    private bool IsSlotEmpty()
    {
        return !PlayerPrefs.HasKey(slotUsedKey);
    }

    private void SaveGame()
    {
        // Use the merged PlayFab manager instance.
        if (PLayFabManager.Instance != null)
        {
            PLayFabManager.Instance.SavePlayerData();

            // Mark slot as used
            PlayerPrefs.SetInt(slotUsedKey, 1);
            PlayerPrefs.Save();

            Debug.Log("Game saved to slot " + slotNumber);
        }
        else
        {
            Debug.LogError("PLayFabManager instance is null!");
        }
    }
}
