using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance { get; set; }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public List<Quest> allActiveQuests;
    public List<Quest> allCompletedQuests;

    [Header("QuestMenu")]
    public GameObject questMenu;
    public bool isQuestMenuOpen;

    public GameObject activeQuestPrefab;
    public GameObject completedQuestPrefab;

    public GameObject questMenuContent;

    [Header("QuestTracker")]
    public GameObject questTrackerContent;
    public GameObject trackerRowPrefab;

    public List<Quest> allTrackedQuests;
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.H) && !isQuestMenuOpen)
        {

            questMenu.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            SelectionManager.Instance.DisableSelection();
            SelectionManager.Instance.GetComponent<SelectionManager>().enabled = false;
            isQuestMenuOpen = true;
            MovementManager.instance.EnableLook(false);

        }
        else if (Input.GetKeyDown(KeyCode.H) && isQuestMenuOpen)
        {
            questMenu.SetActive(false);

            if (!InventorySystem.Instance.isOpen)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                SelectionManager.Instance.EnableSelection();
                SelectionManager.Instance.GetComponent<SelectionManager>().enabled = true;

            }

            isQuestMenuOpen = false;
            MovementManager.instance.EnableLook(true);
        }
    }


    public void TrackQuest(Quest quest)
    {
        allTrackedQuests.Add(quest);
        RefreshTrackerList();
    }
    public void UnTrackQuest(Quest quest)
    {
        allTrackedQuests.Remove(quest);
        RefreshTrackerList();
    }

    public void RefreshTrackerList()
    {
        // Destroying the previous list
        foreach (Transform child in questTrackerContent.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Quest trackedQuest in allTrackedQuests)
        {
            GameObject trackerPrefab = Instantiate(trackerRowPrefab, Vector3.zero, Quaternion.identity);
            trackerPrefab.transform.SetParent(questTrackerContent.transform, false);

            TrackerRow tRow = trackerPrefab.GetComponent<TrackerRow>();

            tRow.questName.text = trackedQuest.questName;
            tRow.description.text = trackedQuest.questDescription;

            var req1 = trackedQuest.info.firstRequirmentItem;
            var req1Amount = trackedQuest.info.firstRequirementAmount;
            var req2 = trackedQuest.info.secondRequirmentItem;
            var req2Amount = trackedQuest.info.secondRequirementAmount;

            if (trackedQuest.info.secondRequirmentItem != "") // if we have 2 requirements
            {
                tRow.requirements.text = $"{req1} " + InventorySystem.Instance.CheckItemAmount(req1) + "/" + $"{req1Amount}\n" +
               $"{req2} " + InventorySystem.Instance.CheckItemAmount(req2) + "/" + $"{req2Amount}\n";
            }
            else // if we have only one
            {
                tRow.requirements.text = $"{req1} " + InventorySystem.Instance.CheckItemAmount(req1) + "/" + $"{req1Amount}\n";
            }

            if (trackedQuest.info.hasCheckPoints)
            {
                var existingText = tRow.requirements.text;
                tRow.requirements.text = PrintCheckpoints(trackedQuest, existingText); 
            }

        }
    }

    private string PrintCheckpoints(Quest trackedQuest, string existingText)
    {
        var finalText = existingText;
        
        foreach (CheckPoints cp in trackedQuest.info.checkPoints)
        {
            if(cp.isCompleted)
            {
                finalText = finalText + "\n" + cp.name + "[Completed]";
            }
            else
            {
                finalText = finalText + "\n" + cp.name;
            }

        }
        return finalText;
    }

    public void AddActiveQuest(Quest quest)
    {
        allActiveQuests.Add(quest);
        TrackQuest(quest);
        RefreshQuestList();
    }

    public void MarkQuestCompleted(Quest quest)
    {
        //remove quest form active list
        allActiveQuests.Remove(quest);
        //add it into the completed list
        allCompletedQuests.Add(quest);
        UnTrackQuest(quest);
        RefreshQuestList();
    }
    public void RefreshQuestList()
    {
        foreach (Transform child in questMenuContent.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Quest activeQuest in allActiveQuests)
        {
            GameObject questPrefab = Instantiate(activeQuestPrefab, Vector3.zero, Quaternion.identity);
            questPrefab.transform.SetParent(questMenuContent.transform, false);

            QuestRow qRow = questPrefab.GetComponent<QuestRow>();

            qRow.thisQuest = activeQuest;

            qRow.questName.text = activeQuest.questName;
            qRow.questGiver.text = activeQuest.questGiver;

            qRow.isActive = true;
            qRow.isTracking = true;

            qRow.coinAmount.text = $"{activeQuest.info.coinReward}";

            if (activeQuest.info.rewardItem1 != "")
            {
                qRow.firstReward.sprite = GetSpriteForitem(activeQuest.info.rewardItem1); ;
                qRow.firstRewardAmound.text = ""; 
            }
            else
            {
                qRow.firstReward.gameObject.SetActive(false);
                qRow.firstRewardAmound.text = "";
            }

            if (activeQuest.info.rewardItem2 != "")
            {
                qRow.secondReward.sprite = GetSpriteForitem(activeQuest.info.rewardItem2); ;
                qRow.secondRewardAmound.text = ""; 
            }
            else
            {
                qRow.secondReward.gameObject.SetActive(false);
                qRow.secondRewardAmound.text = "";
            }
        }

        foreach (Quest completedQuest in allCompletedQuests)
        {
            GameObject questPrefab = Instantiate(completedQuestPrefab, Vector3.zero, Quaternion.identity);
            questPrefab.transform.SetParent(questMenuContent.transform, false);

            QuestRow qRow = questPrefab.GetComponent<QuestRow>();

            qRow.questName.text = completedQuest.questName;
            qRow.questGiver.text = completedQuest.questGiver;

            qRow.isActive = false;
            qRow.isTracking = false;

            qRow.coinAmount.text = $"{completedQuest.info.coinReward}";

            if (completedQuest.info.rewardItem1 != "")
            {
                qRow.firstReward.sprite = GetSpriteForitem(completedQuest.info.rewardItem1); ;
                qRow.firstRewardAmound.text = "";
            }
            else
            {
                qRow.firstReward.gameObject.SetActive(false);
                qRow.firstRewardAmound.text = "";
            }

            if (completedQuest.info.rewardItem2 != "")
            {
                qRow.secondReward.sprite = GetSpriteForitem(completedQuest.info.rewardItem2); ;
                qRow.secondRewardAmound.text = "";
            }
            else
            {
                qRow.secondReward.gameObject.SetActive(false);
                qRow.secondRewardAmound.text = "";
            }
        }

    }

    private Sprite GetSpriteForitem(string item)
    {
        var itemToGet = Resources.Load<GameObject>(item);
        return itemToGet.GetComponent<Image>().sprite;
    }
}

