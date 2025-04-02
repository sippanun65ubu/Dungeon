using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadSlot : MonoBehaviour
{
    public Button loadButton;
    public TextMeshProUGUI loadButtonText;
    public int slotNumber;

    private string slotUsedKey;  
    private string slotTextKey;   

    private void Awake()
    {
        loadButton = GetComponent<Button>();
        loadButtonText = transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();

        // Build the keys for this slot
        slotUsedKey = "SaveSlot_" + slotNumber;
        slotTextKey = "SaveSlotText_" + slotNumber;
    }

    private void Start()
    {
        // Load the button text from PlayerPrefs if available
        if (PlayerPrefs.HasKey(slotTextKey))
        {
            string loadedText = PlayerPrefs.GetString(slotTextKey);
            loadButtonText.text = loadedText;
            Debug.Log($"Slot {slotNumber} text loaded: {loadedText}");
        }
        else
        {
            // If there's no saved text, either show "Empty Slot" or some default text
            loadButtonText.text = "Empty Slot";
            Debug.Log($"Slot {slotNumber} has no saved text in PlayerPrefs.");
        }

        // Add the button click listener
        loadButton.onClick.AddListener(OnLoadButtonClicked);
    }

    private void OnLoadButtonClicked()
    {
        // If the slot is empty, we can't load
        if (IsSlotEmpty())
        {
            loadButtonText.text = "Empty Slot";
            Debug.Log($"Slot {slotNumber} is empty.");
            DeselectButton();
            return;
        }

        // Otherwise, try loading from PlayFab
        loadButtonText.text = "Loading...";
        Debug.Log($"Attempting to load from slot {slotNumber}...");

        // Call your merged PlayFab manager to load data
        if (PLayFabManager.Instance != null)
        {
            PLayFabManager.Instance.LoadPlayerData(loadedData =>
            {
                if (loadedData != null)
                {
                    MovementManager.instance.EnableLook(false);
                    MovementManager.instance.EnableMovement(false);
                    StartCoroutine(PLayFabManager.Instance.SetPlayerDataCoroutine(loadedData));
                    loadButtonText.text = "Game Loaded";
                    Debug.Log($"Game loaded from slot {slotNumber}");
                    MovementManager.instance.EnableLook(false);
                    MovementManager.instance.EnableMovement(false);
                }
                else
                {
                    loadButtonText.text = "No Data Found";
                    Debug.Log($"No saved data found in slot {slotNumber}");
                }
            });
        }
        else
        {
            Debug.LogError("PLayFabManager instance is null! Cannot load data.");
        }

        DeselectButton();
    }

    // Check if the slot is empty by seeing if "SaveSlot_X" is set in PlayerPrefs
    private bool IsSlotEmpty()
    {
        return !PlayerPrefs.HasKey(slotUsedKey);
    }

    // Deselects the button from the EventSystem so it’s not left highlighted
    private void DeselectButton()
    {
        GameObject eventSystem = GameObject.Find("EventSystem");
        if (eventSystem != null)
        {
            eventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
        }
    }
}
