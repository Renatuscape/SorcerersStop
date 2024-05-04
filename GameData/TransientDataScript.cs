using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class TransientDataScript : MonoBehaviour
{
    public static TransientDataScript transientData;
    public static GameManagerScript gameManager;
    public static GameState GameState { get; private set; }

    public Language language;
    public List<GameObject> activePrefabs;

    [TextArea(20, 50)]
    public string gameStateLog = "Game State Changes";

    //void Update() // For debugging collision
    //{
    //    // Cast a ray from the camera to the mouse position
    //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //    RaycastHit hit;

    //    // Perform the raycast
    //    if (Physics.Raycast(ray, out hit))
    //    {
    //        // Check if the hit object has a collider
    //        if (hit.collider != null)
    //        {
    //            // Log the name of the collided GameObject
    //            Debug.Log("Mouse is hovering over: " + hit.collider.gameObject.name);
    //        }
    //    }
    //}

    //CAMERA
    public static CameraView CameraView { get; private set; }

    //UI FUNCTIONS
    public PushAlertManager pushAlertManager;
    public MenuFloatTextScript floatText;

    //COACH DATA
    public float currentSpeed;
    public float currentMana;
    public float manapool;
    public float engineBoost;
    public EngineState engineState;

    //PLAYER LOCATION DATA
    public Region currentRegion;
    public Location currentLocation = null;

    //TIME DATA
    public int timeFlowSpeed = 1;
    public DayOfWeek weekDay;
    public int date;
    public int month;
    public int year;

    //UI CONTENT
    public string mouseToolTip;
    public string infoBox;


    private void Awake()
    {
        transientData = this;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
    }

    //*** PUBLIC METHODS ***
    //UI TEXT

    public void OnLocationSpawn(Location location)
    {
        Debug.Log($"Ready to spawn {location.name} {location.objectID} when logic is implemented.");

        if (GameState == GameState.Overworld)
        {
            gameManager.questTracker.RunCheck();
        }
    }

    //SYSTEM METHODS
    public void PurgePrefabs()
    {
        foreach (GameObject prefab in activePrefabs)
        {
            if (prefab != null)
            {
                Destroy(prefab);
            }
        }
    }
    //NEW STATIC METHODS
    public static GameState GetGameState()
    {
        return GameState;
    }
    public static void SetCameraView(CameraView view)
    {
        CameraView = view;
    }

    public static void ReturnToOverWorld(string name, GameObject gameObject)
    {
        SetGameState(GameState.Overworld, name, gameObject);
    }

    public static void TravelByGate(Gate gate)
    {
        Debug.Log("Travel by Gate called at TransientDataScript");
        GameObject.Find("GameManager").GetComponent<GameManagerScript>().mapComponent.TravelByGate(gate);
        AudioManager.FadeToStop();
    }

    public static bool IsTimeFlowing() //in this state, mana regenerates and plants grow. Controllers are enabled
    {
        if (GameState == GameState.Overworld
            || GameState == GameState.JournalMenu
            || GameState == GameState.BankMenu
            || GameState == GameState.ShopMenu
            || GameState == GameState.Dialogue // Used by pop dialogue and memories. Disables floating text.
            || GameState == GameState.StartMenu
            || GameState == GameState.AlchemyMenu)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    static void LogStateChange(string callerScript, GameObject callerObject, GameState newState)
    {
        transientData.gameStateLog += "\n" + Time.realtimeSinceStartup + ": " + callerScript + "(script) on " + callerObject.name + "(game object) changed the game state from " + GameState + " to " + newState + ".";
    }

    public static void PushAlert(string alert)
    {
        if (NullCheck())
        {
            transientData.pushAlertManager.QueueAlert(alert);
        }
        else
        {
            Debug.Log("transientData not found.");
        }
    }

    public static void PrintFloatText(string content)
    {
        if (NullCheck())
        {
            if (GameState != GameState.MainMenu && GameState != GameState.Loading && GameState != GameState.Dialogue)
            {
                transientData.floatText.PrintFloatText(content);
            }
        }
        else
        {
            Debug.Log("transientData not found.");
        }
    }

    public static void PrintFloatEmbellishedItem(Item item, bool printPrice, bool printRarity)
    {
        if (NullCheck())
        {
            Items.PrintFloatEmbellishedItem(item, printPrice, printRarity);
        }
        else
        {
            Debug.Log("transientData not found.");
        }
    }

    public static void DisableFloatText()
    {
        if (NullCheck())
        {
            transientData.floatText.DisableFloatText();
        }
        else
        {
            Debug.Log("transientData not found.");
        }
    }

    public static float GetTimeOfDay()
    {
        if (NullCheck())
        {
            return gameManager.dataManager.timeOfDay;
        }
        else
        {
            Debug.Log("gameManager not found.");
            return 0.6f;
        }
    }

    public static Location GetCurrentLocation()
    {
        if (NullCheck())
        {
            return transientData.currentLocation;
        }
        else
        {
            Debug.Log("transientData not found.");
            return null;
        }
    }

    public static void SetGameState(GameState newState, string callerScript, GameObject callerObject)
    {
        if (NullCheck())
        {
            LogStateChange(callerScript, callerObject, newState);
            GameState = newState;
            DisableFloatText();

            if (newState == GameState.Overworld)
            {
                gameManager.questTracker.RunCheck();
            }
        }
        else
        {
            Debug.Log("transientData not found.");
        }
    }

    public static DayOfWeek GetWeekDay()
    {
        if (NullCheck())
        {
            return transientData.weekDay;
        }
        else
        {
            Debug.Log("transientData not found.");
            return DayOfWeek.Solden;
        }
    }

    public static MenuSystem GetMenuSystem()
    {
        if (NullCheck())
        {
            Debug.Log($"Returning menu system ({gameManager.menuSystem})");
            return gameManager.menuSystem;
        }
        else
        {
            Debug.Log("transientData not found.");
            return null;
        }
    }

    public static StorySystem GetStorySystem()
    {
        if (NullCheck())
        {
            Debug.Log($"Returning dialogue system ({gameManager.storySystem})");
            return gameManager.storySystem;
        }
        else
        {
            Debug.Log("transientData not found.");
            return null;
        }
    }

    public static TransientDataScript GetTransientData()
    {
        if (NullCheck())
        {
            return transientData;
        }
        else
        {
            Debug.Log("transientData not found.");
            return null;
        }
    }

    private static bool NullCheck()
    {
        if (transientData == null)
        {
            transientData = GameObject.Find("TransientData").GetComponent<TransientDataScript>();
        }

        if (transientData != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static void DailyTick()
    {
        QuestResetter.Tick();
        Player.CheckTimers();
        CharacterNodeTracker.UpdateNodesOnDayTick();

        if (GetDaysPassed() % 7 == 0)
        {
            if (gameManager.dataManager.giftedThisWeek != null)
            {
                gameManager.dataManager.giftedThisWeek.Clear();
            }
        }


    }

    public static bool GiftCheck(Character character)
    {
        return gameManager.dataManager.giftedThisWeek.Contains(character.objectID);
    }

    public static void SetAsGifted(Character character)
    {
        gameManager.dataManager.giftedThisWeek.Add(character.objectID);
    }

    public static int GetDaysPassed()
    {
        return gameManager.dataManager.totalGameDays;
    }
}


public static class MouseTracker
{
    public static Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 0));

        return worldPosition;
    }
}
