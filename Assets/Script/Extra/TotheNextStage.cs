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
    private bool hasActivatedChild = false;

    void Update()
    {
        if (hasActivatedChild)
            return;

        if (GameManager.instance != null
         && GameManager.instance.totalScore >= requiredScore)
        {
            int childCount = transform.childCount;
            if (childCount == 0) return;

            // pick one at random
            int idx = Random.Range(0, childCount);
            Transform chosen = transform.GetChild(idx);

            // activate only the chosen one
            chosen.gameObject.SetActive(true);

            //make sure the others stay inactive
            for (int i = 0; i < childCount; i++)
            {
                if (i != idx)
                    transform.GetChild(i).gameObject.SetActive(false);
            }

            hasActivatedChild = true;
            Debug.Log($"Activated child #{idx} ({chosen.name}) because score = {GameManager.instance.totalScore}");
        }
    }


    public void ChildTriggerEntered(Collider other)
    {
        if (other.CompareTag("Player") && hasActivatedChild)
        {
            Debug.Log("Loading next scene: " + nextSceneName);
            SceneManager.LoadScene(nextSceneName);
        }
    }
}

