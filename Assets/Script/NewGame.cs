using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static PLayFabManager;

public class NewGame : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private TextMeshProUGUI statusText;
    //[Header("New Game Settings")]
    //[SerializeField] private string startingScene = "TownNo2";

    private void Start()
    {
        if (newGameButton != null)
        {
            newGameButton.onClick.AddListener(OnNewGameButtonClicked);
        }
    }

    private void OnNewGameButtonClicked()
    {
        PlayerData defaultData = PlayerData.CreateDefaultData();

        // Start the coroutine to apply the default data.
        if (PLayFabManager.Instance != null)
        {
            //StartCoroutine(PLayFabManager.Instance.SetGameDataCoroutine(defaultData));
            StartCoroutine(MenuManager.Instance.ClosesMenu());


        }
        else
        {
            Debug.LogError("PLayFabManager instance is null! Cannot start new game.");
        }
    }
}
