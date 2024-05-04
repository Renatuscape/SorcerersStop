using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;
public static class Player
{
    public static List<IdIntPair> inventoryList = new();

    public static bool GetEntry(string objectID, string caller, out IdIntPair entry)
    {
        entry = inventoryList?.Find(o => o?.objectID != null && o.objectID.Contains(objectID));

        if (entry != null)
        {
            return true;
        }
        return false;
    }

    // REMOVING - makes sure a positive number is flipped to negative
    public static void Remove(IdIntPair entry, bool doNotLog = false)
    {
        Remove(entry.objectID, entry.amount, doNotLog);
    }
    public static void Remove(string objectID, int amount = 1, bool doNotLog = false)
    {
        int removeAmount = amount;

        if (removeAmount > 0)
        {
            removeAmount = removeAmount * -1;
        }

        Add(objectID, removeAmount, doNotLog);
    }

    // SET - forcibly sets object to specified value in player inventory
    public static void Set(string objectID, int amount, bool doNotLog = false)
    {
        var foundObject = inventoryList.FirstOrDefault(o => o.objectID == objectID);

        if (foundObject != null)
        {
            inventoryList.FirstOrDefault(o => o.objectID == objectID).amount = amount;
        }
        else
        {
            Add(objectID, amount, doNotLog);
        }
    }

    // ADDING METHODS - positve numbers are added from inventory, negative numbers are removed
    public static int Add(IdIntPair entry, bool doNotLog = false)
    {
        if (!string.IsNullOrEmpty(entry.description)) // If the entry contains a description, override any other possible log.
        {
            LogAlert.QueueTextAlert(entry.description);
            return Add(entry.objectID, entry.amount, true);
        }

        return Add(entry.objectID, entry.amount, doNotLog);
    }

    public static int Add(string objectID, int amount = 1, bool doNotLog = false)
    {
        if (objectID.Contains("-NAME"))
        {
            var dataManager = TransientDataScript.gameManager.dataManager;

            Debug.Log($"Detected -NAME tag. Adding {objectID} to list.");
            dataManager.unlockedNames.Add(objectID);
            DialogueTagParser.UpdateTags(dataManager);
            Debug.Log("Name unlocked. Updating tags.");

            return 1;
        }
        else if (objectID.Contains("-ROMANCE"))
        {
            AddRomance(objectID, amount);
            return amount;
        }
        else if (objectID.Contains("-SetTIMER"))
        {
            AddTimer(objectID);
            return 0;
        }

        return FindObjectAndAddUpToMax(objectID, amount, doNotLog);
    }

    static void AddTimer(string objectID)
    {
        Debug.Log($"Detected -SetTIMER tag. Adding {objectID} to inventory.");
        var foundEntry = inventoryList.FirstOrDefault(e => e.objectID == objectID);

        if (foundEntry != null)
        {
            Debug.Log("Found existing timer entry. Resetting timer to 0.");
            foundEntry.amount = 0;
        }
        else
        {
            Debug.Log("No existing timer entry. Creating and adding new.");
            foundEntry = new() { objectID = objectID, amount = 0 };
            inventoryList.Add(foundEntry);
        }
    }

    static void AddRomance(string objectID, int amount)
    {
        Debug.Log($"Detected -ROMANCE tag. Adding {objectID} to inventory.");
        var foundEntry = inventoryList.FirstOrDefault(e => e.objectID == objectID);

        if (foundEntry != null)
        {
            Debug.Log("Found existing romance entry. Adding amount.");
            foundEntry.amount += amount; // allow negative numbers. No max needed.
        }
        else
        {
            Debug.Log("No existing romance entry. Creating and adding new.");
            foundEntry = new() { objectID = objectID, amount = amount };
            inventoryList.Add(foundEntry);
        }
    }

    static int FindObjectAndAddUpToMax(string objectID, int amount, bool doNotLog = false)
    {
        int amountAdded = 0;

        // Check whether object exists and get its base data
        var foundObject = GameCodex.GetBaseObject(objectID);

        if (foundObject != null)
        {
            // Check whether object already exists in inventory. GetCount is not sufficient here
            var entry = inventoryList.FirstOrDefault(e => e.objectID == objectID);

            // Create new entry if one does not exist
            if (entry == null)
            {
                entry = new() { objectID = objectID, amount = 0 };
                inventoryList.Add(entry);
            }

            if (entry.amount + amount <= foundObject.maxValue)
            {
                entry.amount += amount;
                amountAdded = amount;
            }
            else
            {
                entry.amount = foundObject.maxValue;
                amountAdded = foundObject.maxValue - entry.amount;
            }

        }

        if (!doNotLog)
        {
            AttemptToLog(foundObject, amountAdded);
        }

        return amountAdded;
    }

    static void AttemptToLog(BaseObject baseObject, int value)
    {
        if (baseObject.objectType == ObjectType.Item)
        {
            LogAlert.QueueItemAlert((Item)baseObject, value);
        }
        else if (baseObject.objectType == ObjectType.Character)
        {
            LogAlert.QueueAffectionAlert((Character)baseObject, value);
        }
        else if (baseObject.objectType == ObjectType.Skill)
        {
            var skill = (Skill)baseObject;
            LogAlert.QueueTextAlert($"{skill.name} increased by {value}");
        }
        else if (baseObject.objectType == ObjectType.Recipe)
        {
            var recipe = (Recipe)baseObject;

            if (value < 0)
            {
                LogAlert.QueueTextAlert($"Removed {recipe.name} from recipe tin.");
            }
            else
            {
                LogAlert.QueueTextAlert($"Obtained {recipe.name}!");
            }
        }
        else if (baseObject.objectType == ObjectType.Upgrade)
        {
            var upgrade = (Upgrade)baseObject;

            if (value < 0)
            {
                LogAlert.QueueTextAlert($"{upgrade.name} wore down.");
            }
            else
            {
                LogAlert.QueueTextAlert($"{upgrade.name} upgraded!");
            }
        }
    }

    public static int GetCount(string searchID, string caller)
    {
        //Debug.Log(caller + " called Player.GetCount()");
        if (GetEntry(searchID, "Player", out var entry))
        {
            return entry.amount;
        }
        else
        {
            return 0;
        }
    }

    public static void CheckTimers()
    {
        Debug.Log("Checking timers.");
        List<IdIntPair> timersToRemove = new();
        foreach (var entry in inventoryList)
        {
            if (entry.objectID.Contains("-SetTIMER"))
            {
                var timerData = entry.objectID.Split('-');

                if (int.TryParse(timerData[4], out int timerMax))
                {
                    Debug.Log("This timer is set for " + timerMax);

                    if (entry.amount < timerMax)
                    {
                        Debug.Log("Updated quest timer " + entry.objectID);
                        entry.amount++;
                    }
                    else
                    {
                        Debug.Log("Quest timer has reached max. Checking for quest completion.");

                        Quest relevantQuest = Quests.FindByID(timerData[0] + "-" + timerData[1]);
                        if (relevantQuest == null)
                        {
                            Debug.LogWarning("Could not find relevant quest related to " + entry.objectID);
                        }
                        else
                        {
                            if (int.TryParse(timerData[2], out int timerStage))
                            {
                                int stage = GetCount(relevantQuest.objectID, "Player");

                                if (stage > timerStage)
                                {
                                    Debug.Log("Timed quest completed. Removing timer.");
                                    timersToRemove.Add(entry);
                                }
                            }
                        }
                    }
                }
            }
        }

        foreach (var entry in timersToRemove)
        {
            inventoryList.Remove(entry);
        }
    }

    public static List<Item> GetInventory()
    {
        return Items.all.Where(i => GetCount(i.objectID, "Player.GetInventory()") > 0).ToList();
    }
}