using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum StageType
{
    Dialogue,
    Memory,
    PopUp,
    Node
}

[System.Serializable]
public class Dialogue
{
    public StageType stageType;
    public string objectID;
    public string questID;
    public string speakerID;
    public int questStage;
    public string topicName;
    public string hint;
    public int startTime = 0;
    public int endTime = 0;
    public int minimumDaysPassed;

    public List<string> content;
    public List<DialogueStep> dialogueSteps = new(); //a step is one line of dialogue from one speaker
    public List<IdIntPair> requirements = new();
    public List<IdIntPair> restrictions = new();
    public List<Choice> choices = new(); //LEAVE CHOICES BLANK TO LOOP DIALOGUE WITHOUT PROGRESSION
    public bool autoProgressStage = false; //automatically go to next stage when step is completed
    public bool noLeaveButton = true; //if this is enabled, it is impossible to leave dialogue until you reach a choice

    //MEMORY SPECIFIC
    public string location;

    public bool CheckRequirements(out bool hasTimer, out bool hasLocation)
    {
        hasTimer = false;
        hasLocation = false;

        if ((startTime != 0 && endTime != 0) || minimumDaysPassed > 0)
        {
            hasTimer = true;
        }

        if (!string.IsNullOrEmpty(location))
        {
            hasLocation = true;
        }

        if (requirements != null && requirements.Count > 0)
        {
            Debug.Log($"Checking requirements for {objectID}.");

            foreach (IdIntPair requirement in requirements)
            {
                int amount = Player.GetCount(requirement.objectID, "Choice Requirement Check");

                Debug.Log($"{requirement.amount} {requirement.objectID} is required. Player has {amount}");

                if (amount < requirement.amount)
                {
                    return false;
                }

                Debug.Log("Returned true.");
            }
        }

        if (restrictions != null && restrictions.Count > 0) //don't run if checks already failed
        {
            foreach (IdIntPair restriction in restrictions)
            {
                int amount = Player.GetCount(restriction.objectID, "Choice Restriction Check");
                Debug.Log($"{restriction.objectID} is restricted to max {restriction.amount}. Player has {amount}.");
                if (amount > restriction.amount)
                {
                    Debug.Log("Returned false.");
                    return false;
                }
            }
            Debug.Log("Returned true.");
        }

        return true;
    }
}

public static class Dialogues
{
    public static List<Dialogue> all = new();

    public static void DebugList()
    {
        Debug.LogWarning("Dialogue.DebugList() called");

        foreach (Dialogue dialogue in all)
        {
            Debug.Log($"Dialogue for quest {dialogue.questID}: stage {dialogue.questStage}");
        }
    }

    public static void DebugAllItems()
    {
        foreach (Dialogue dialogue in all)
        {
            Debug.Log($"{dialogue}" +
                $"\n{dialogue.dialogueSteps.Count}");
        }
    }

    public static List<Dialogue> FindQuestDialogues(string questID)
    {
        return all
            .Where(d => d.questID == questID)
            .OrderBy(d => d.questStage)
            .ToList();
    }
}