using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static PLayFabManager;

public class NPC : MonoBehaviour
{

    public bool playerInRange;
    public bool isTalkingWithPlayer;
    public string npcId;

    public TextMeshProUGUI npcDialogText;

    public Button optionButton1;
    public TextMeshProUGUI optionButton1Text;

    public Button optionButton2;
    public TextMeshProUGUI optionButton2Text;

    public List<Quest> quests;
    public Quest currentActiveQuest = null;
    public int activeQuestIndex = 0;
    public bool firstTimeInteraction = true;
    public int currentDialog;

    public void Start()
    {
        npcDialogText = DialogSystem.instance.dialogText;

        optionButton1 = DialogSystem.instance.option1BTN;
        optionButton1Text = DialogSystem.instance.option1BTN.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();

        optionButton2 = DialogSystem.instance.option2BTN;
        optionButton2Text = DialogSystem.instance.option2BTN.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
    }
    public void OnTriggerEnter(Collider other)
    {


        if (other.CompareTag("Player"))
        {

            playerInRange = true;


        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            playerInRange = false;


        }
    }
    public void StartConversation()
    {
        isTalkingWithPlayer = true;

        LookAtPlayer();

        // Interacting with the NPC for the first time
        if (firstTimeInteraction)
        {
            firstTimeInteraction = false;
            currentActiveQuest = quests[activeQuestIndex]; // 0 at start
            StartQuestInitialDialog();
            currentDialog = 0;
        }
        else // Interacting with the NPC after the first time
        {

            // If we return after declining the quest
            if (currentActiveQuest.declined)
            {

                DialogSystem.instance.OpenDialogUI();

                npcDialogText.text = currentActiveQuest.info.comebackAfterDecline;

                SetAcceptAndDeclineOptions();
            }


            // If we return while the quest is still in progress
            if (currentActiveQuest.accepted && currentActiveQuest.isCompleted == false)
            {
                if (AreQuestRequirmentsCompleted())
                {

                    SubmitRequiredItems();

                    DialogSystem.instance.OpenDialogUI();

                    npcDialogText.text = currentActiveQuest.info.comebackCompleted;

                    optionButton1Text.text = "[Take Reward]";
                    optionButton1.onClick.RemoveAllListeners();
                    optionButton1.onClick.AddListener(() => {
                        ReceiveRewardAndCompleteQuest();
                    });
                }
                else
                {
                    DialogSystem.instance.OpenDialogUI();

                    npcDialogText.text = currentActiveQuest.info.comebackInProgress;

                    optionButton1Text.text = "[Close]";
                    optionButton1.onClick.RemoveAllListeners();
                    optionButton1.onClick.AddListener(() => {
                        DialogSystem.instance.CloseDialogUI();
                        isTalkingWithPlayer = false;
                    });
                }
            }

            if (currentActiveQuest.isCompleted == true)
            {
                DialogSystem.instance.OpenDialogUI();

                npcDialogText.text = currentActiveQuest.info.finalWords;

                optionButton1Text.text = "[Close]";
                optionButton1.onClick.RemoveAllListeners();
                optionButton1.onClick.AddListener(() => {
                    DialogSystem.instance.CloseDialogUI();
                    isTalkingWithPlayer = false;
                });
            }

            // If there is another quest available
            if (currentActiveQuest.initialDialogCompleted == false)
            {
                StartQuestInitialDialog();
            }

        }

    }

    public void SetAcceptAndDeclineOptions()
    {
        optionButton1Text.text = currentActiveQuest.info.acceptOption;
        optionButton1.onClick.RemoveAllListeners();
        optionButton1.onClick.AddListener(() => {
            AcceptedQuest();
        });

        optionButton2.gameObject.SetActive(true);
        optionButton2Text.text = currentActiveQuest.info.declineOption;
        optionButton2.onClick.RemoveAllListeners();
        optionButton2.onClick.AddListener(() => {
            DeclinedQuest();
        });
    }

    public void SubmitRequiredItems()
    {
        string firstRequiredItem = currentActiveQuest.info.firstRequirmentItem;
        int firstRequiredAmount = currentActiveQuest.info.firstRequirementAmount;

        if (firstRequiredItem != "")
        {
            InventorySystem.Instance.RemoveItem(firstRequiredItem, firstRequiredAmount);
        }


        string secondtRequiredItem = currentActiveQuest.info.secondRequirmentItem;
        int secondRequiredAmount = currentActiveQuest.info.secondRequirementAmount;

        if (firstRequiredItem != "")
        {
            InventorySystem.Instance.RemoveItem(secondtRequiredItem, secondRequiredAmount);
        }

    }

    public bool AreQuestRequirmentsCompleted()
    {
        print("Checking Requirments");

        // First Item Requirment

        string firstRequiredItem = currentActiveQuest.info.firstRequirmentItem;
        int firstRequiredAmount = currentActiveQuest.info.firstRequirementAmount;

        var firstItemCounter = 0;

        foreach (string item in InventorySystem.Instance.itemList)
        {
            if (item == firstRequiredItem)
            {
                firstItemCounter++;
            }
        }

        // Second Item Requirment -- If we dont have a second item, just set it to 0

        string secondRequiredItem = currentActiveQuest.info.secondRequirmentItem;
        int secondRequiredAmount = currentActiveQuest.info.secondRequirementAmount;

        var secondItemCounter = 0;

        foreach (string item in InventorySystem.Instance.itemList)
        {
            if (item == secondRequiredItem)
            {
                secondItemCounter++;
            }
        }

        SetQuestHasCheckPoints(currentActiveQuest);

        bool allCheckpointsCompleted = false;

        if (currentActiveQuest.info.hasCheckPoints)
        {
            foreach (CheckPoints cp in currentActiveQuest.info.checkPoints)
            {
                if (cp.isCompleted ==  false)
                {
                    allCheckpointsCompleted = false;
                    break;
                }
                allCheckpointsCompleted = true;
            }
        }



        if (firstItemCounter >= firstRequiredAmount && secondItemCounter >= secondRequiredAmount)
        {
            if (currentActiveQuest.info.hasCheckPoints) // if we have checkpoint
            {
                if (allCheckpointsCompleted) // if item and requirement are completed
                {
                    return  true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }
        else
        {
            return false;
        }
    }

    public void SetQuestHasCheckPoints(Quest activeQuest)
    {
        if (currentActiveQuest.info.checkPoints.Count > 0)
        {
            currentActiveQuest.info.hasCheckPoints = true;
        }
        else
        {
            currentActiveQuest.info.hasCheckPoints = false;
        }
    }

    public void StartQuestInitialDialog()
    {
        DialogSystem.instance.OpenDialogUI();

        npcDialogText.text = currentActiveQuest.info.initialDialog[currentDialog];
        optionButton1Text.text = "Next";
        optionButton1.onClick.RemoveAllListeners();
        optionButton1.onClick.AddListener(() => {
            currentDialog++;
            CheckIfDialogDone();
        });

        optionButton2.gameObject.SetActive(false);
    }

    public void CheckIfDialogDone()
    {
        if (currentDialog == currentActiveQuest.info.initialDialog.Count - 1) // If its the last dialog 
        {
            npcDialogText.text = currentActiveQuest.info.initialDialog[currentDialog];

            currentActiveQuest.initialDialogCompleted = true;

            SetAcceptAndDeclineOptions();
        }
        else  // If there are more dialogs
        {
            npcDialogText.text = currentActiveQuest.info.initialDialog[currentDialog];

            optionButton1Text.text = "Next";
            optionButton1.onClick.RemoveAllListeners();
            optionButton1.onClick.AddListener(() => {
                currentDialog++;
                CheckIfDialogDone();
            });
        }
    }
    public void AcceptedQuest()
    {
        QuestManager.instance.AddActiveQuest(currentActiveQuest);
         
        currentActiveQuest.accepted = true;
        currentActiveQuest.declined = false;

        if (currentActiveQuest.hasNoRequirements)
        {
            npcDialogText.text = currentActiveQuest.info.comebackCompleted;
            optionButton1Text.text = "[Take Reward]";
            optionButton1.onClick.RemoveAllListeners();
            optionButton1.onClick.AddListener(() => {
                ReceiveRewardAndCompleteQuest();
            });
            optionButton2.gameObject.SetActive(false);
        }
        else
        {
            npcDialogText.text = currentActiveQuest.info.acceptAnswer;
            CloseDialogUI();
        }



    }

    public void CloseDialogUI()
    {
        optionButton1Text.text = "[Close]";
        optionButton1.onClick.RemoveAllListeners();
        optionButton1.onClick.AddListener(() => {
            DialogSystem.instance.CloseDialogUI();
            isTalkingWithPlayer = false;
        });
        optionButton2.gameObject.SetActive(false);
    }

    public void ReceiveRewardAndCompleteQuest()
    {
        QuestManager.instance.MarkQuestCompleted(currentActiveQuest);

        currentActiveQuest.isCompleted = true;

        var coinsRecieved = currentActiveQuest.info.coinReward;
        print("You recieved " + coinsRecieved + " gold coins");

        if (currentActiveQuest.info.rewardItem1 != "")
        {
            InventorySystem.Instance.AddToInventory(currentActiveQuest.info.rewardItem1);
        }

        if (currentActiveQuest.info.rewardItem2 != "")
        {
            InventorySystem.Instance.AddToInventory(currentActiveQuest.info.rewardItem2);
        }

        activeQuestIndex++;

        // Start Next Quest 
        if (activeQuestIndex < quests.Count)
        {
            currentActiveQuest = quests[activeQuestIndex];
            currentDialog = 0;
            DialogSystem.instance.CloseDialogUI();
            isTalkingWithPlayer = false;
        }
        else
        {
            DialogSystem.instance.CloseDialogUI();
            isTalkingWithPlayer = false;
            print("No more quests");
        }

    }

    public void DeclinedQuest()
    {
        currentActiveQuest.declined = true;

        npcDialogText.text = currentActiveQuest.info.declineAnswer;
        CloseDialogUI();
    }

    public void LookAtPlayer()
    {
        var player = PlayerState.Instance.playerBody.transform;
        Vector3 direction = player.position - transform.position;
        transform.rotation = Quaternion.LookRotation(direction);

        var yRotation = transform.eulerAngles.y;
        transform.rotation = Quaternion.Euler(0,yRotation, 0);
    }
    public void ResetToDefaults()
    {
        isTalkingWithPlayer = false;
        firstTimeInteraction = true;
        activeQuestIndex = 0;
        currentDialog = 0;
        currentActiveQuest = null;
        DialogSystem.instance?.CloseDialogUI();
    }
}
