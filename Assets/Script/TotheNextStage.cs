using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TotheNextStage : MonoBehaviour
{
    [Tooltip("Score required to activate the next stage object (child).")]
    public int requiredScore = 1000;
    public string nextSceneName;

    void Update()
    {
        // Ensure GameManager exists.
        if (GameManager.instance != null && GameManager.instance.totalScore >= requiredScore)
        {
            // Activate all child GameObjects if they are not already active.
            foreach (Transform child in transform)
            {
                if (!child.gameObject.activeSelf)
                {
                    child.gameObject.SetActive(true);
                }
            }
            Debug.Log("All children activated because score (" + GameManager.instance.totalScore + ") reached threshold (" + requiredScore + ").");
        }
    }

    /// <summary>
    /// This method is called by the child's TriggerForwarder script.
    /// </summary>
    public void ChildTriggerEntered(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Check if all children are active.
            if (AreAllChildrenActive())
            {
                Debug.Log("Player entered collider and all children are active. Loading next scene: " + nextSceneName);
                SceneManager.LoadScene(nextSceneName);
            }
        }
    }

    // Helper method to check if all child objects are active.
    private bool AreAllChildrenActive()
    {
        foreach (Transform child in transform)
        {
            if (!child.gameObject.activeSelf)
                return false;
        }
        return true;
    }
}
