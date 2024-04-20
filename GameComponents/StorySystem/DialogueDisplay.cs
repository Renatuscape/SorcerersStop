using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;

public class DialogueDisplay : MonoBehaviour
{
    public DialogueMenu dialogueMenu;
    public DialoguePortraitManager portraitManager;

    public GameObject dialogueContainer;

    public GameObject leftNameDisplay;
    public GameObject rightNameDisplay;

    public TextMeshProUGUI chatHistory;

    public TextMeshProUGUI contentText;
    public TextMeshProUGUI leftNameText;
    public TextMeshProUGUI rightNameText;

    public Button btnSpeed;
    public Button btnAutoPlay;


    public Dialogue activeDialogue;
    public DialogueEvent activeEvent;
    public bool continueEnabled;
    public bool isPrinting;
    public bool autoEnabled;
    public bool endConversation;
    public bool readyToPrintChoices;
    public bool readyToContinue;
    public int eventIndex;

    public float printSpeed = 0.05f;

    public float autoDelay = 2;
    public float autoTimer;
    private void Start()
    {
        btnAutoPlay.onClick.AddListener(() => ToggleAuto());
        chatHistory.text = "<b>Conversation History</b>\n";
    }

    private void Update()
    {
        if (autoEnabled && !isPrinting)
        {
            autoTimer += Time.deltaTime;

            if (autoTimer > autoDelay)
            {
                if (eventIndex < activeDialogue.dialogueEvents.Count)
                {
                    autoTimer = 0;
                    PrintEvent();
                }
            }
        }

        if (readyToPrintChoices &&  !isPrinting)
        {
            dialogueMenu.PrintChoices(activeDialogue);

            readyToPrintChoices = false;
            continueEnabled = false;
            readyToContinue = false;
        }
    }

    // Concerns only the display of text. Portraits are handled by DialoguePortraitManager
    // DialogueMenu handles the quest and quest progression
    public void StartDialogue(Dialogue dialogue)
    {
        continueEnabled = false;
        endConversation = false;
        readyToPrintChoices = false;
        readyToContinue = false;
        isPrinting = false;

        gameObject.SetActive(true);

        activeDialogue = dialogue;
        eventIndex = 0;

        PrintEvent();
    }

    public void PrintEvent()
    {
        activeEvent = activeDialogue.dialogueEvents[eventIndex];
        SetDisplayNames(activeEvent);

        // Parse tags here instead of at start to get latest tags
        var parsedText = DialogueTagParser.ParseText(activeEvent.content);
        StartCoroutine(PrintContent(parsedText));
        chatHistory.text += parsedText + "\n";

        eventIndex++;

        if (eventIndex >= activeDialogue.dialogueEvents.Count)
        {
            continueEnabled = false;


            if (activeDialogue.choices == null || activeDialogue.choices.Count == 0)
            {
                endConversation = true;
            }
            else
            {
                readyToPrintChoices = true;
            }
        }
        else if (!autoEnabled)
        {
            continueEnabled = true;
        }

        Canvas.ForceUpdateCanvases();
        portraitManager.StartDialogueEvent(activeEvent);
    }

    public void PrintChoiceResult(Choice choice, bool isSuccess, List<IdIntPair> missingItems)
    {
        string speakerTag = isSuccess ? choice.successSpeaker : choice.failureSpeaker;

        if (!string.IsNullOrEmpty(speakerTag)) // if there is no speaker, skip the print
        {
            DialogueEvent resultEvent = new();

            resultEvent.speaker = Characters.FindByTag(speakerTag, name);

            if (resultEvent.speaker == null)
            {
                Debug.LogWarning("Something was wrong with speaker ID for choice in " + activeDialogue.objectID);
            }
            else
            {
                string content = isSuccess ? choice.successText : choice.failureText;

                SetDisplayNames(resultEvent);
                var parsedText = DialogueTagParser.ParseText(content);
                StartCoroutine(PrintContent(parsedText));

                PrintToChatLog("Choice: " + choice.optionText, true, true);
                PrintToChatLog(resultEvent.speaker.NamePlate(), true, false);
                PrintToChatLog(parsedText, false, false);
            }
        }
        else if (choice.endConversation)
        {
            endConversation = true;

            if (missingItems == null || missingItems.Count == 0)
            {
                dialogueMenu.EndDialogue();
            }
        }

        if (missingItems != null && missingItems.Count > 0)
        {
            foreach (var entry in missingItems)
            {
                Debug.Log($"Missing {entry.amount} {entry.objectID}. Print this for the player somehow.");
            }
        }

        if (!isSuccess || choice.endConversation)
        {
            endConversation = true;
        }
        else
        {
            readyToContinue = true;
        }
    }

    IEnumerator PrintContent(string textToPrint)
    {
        printSpeed = 0.05f;
        isPrinting = true;
        contentText.text = "";

        var textArray = textToPrint.Split(' ');

        foreach (var text in textArray)
        {
            //if (printSpeed > 0)
            //{
            //    AudioManager.PlayAmbientSound("knockSmall", -0.1f);
            //}

            if (printSpeed == 0)
            {
                contentText.text = textToPrint;
                break;
            }

            yield return new WaitForSeconds(printSpeed);
            contentText.text += text + " ";
        }

        isPrinting = false;
    }

    void SetDisplayNames(DialogueEvent dEvent)
    {
        if (dEvent.speaker.objectID == "ARC999")
        {
            rightNameDisplay.gameObject.SetActive(false);
            leftNameDisplay.gameObject.SetActive(false);
        }
        else
        {
            if (dEvent.isLeft || dEvent.speaker.objectID == "ARC000")
            {
                leftNameText.text = dEvent.speaker.NamePlate();
                leftNameDisplay.gameObject.SetActive(true);
                rightNameDisplay.gameObject.SetActive(false);
            }
            else
            {
                rightNameText.text = dEvent.speaker.NamePlate();
                rightNameDisplay.SetActive(true);
                leftNameDisplay.gameObject.SetActive(false);
            }

            PrintToChatLog(dEvent.speaker.NamePlate(), true, false);
        }
    }

    public void Continue()
    {
        if (continueEnabled && !isPrinting)
        {
            PrintEvent();
        }
        else if (isPrinting)
        {
            //AudioManager.PlayAmbientSound("smallSnap");
            printSpeed = 0;
        }
        else if (endConversation)
        {
            portraitManager.gameObject.SetActive(false);
            dialogueMenu.EndDialogue();
        }
        else if (readyToContinue && !isPrinting)
        {
            dialogueMenu.ContinueAfterChoice();
        }
    }

    public void ToggleAuto()
    {
        autoEnabled = !autoEnabled;
    }

    public void PrintToChatLog(string text, bool spaceBefore, bool italics = false)
    {
        if (spaceBefore)
        {
            chatHistory.text += "\n";
        }

        Canvas.ForceUpdateCanvases();

        if (italics)
        {
            chatHistory.text += "<i>";
        }

        Canvas.ForceUpdateCanvases();

        chatHistory.text += text;


        if (italics)
        {
            chatHistory.text += "</i>";
        }

        chatHistory.text += "\n";

        chatHistory.gameObject.GetComponent<ContentSizeFitter>().enabled = false;
        chatHistory.gameObject.GetComponent<ContentSizeFitter>().enabled = true;
        Canvas.ForceUpdateCanvases();
    }
}
