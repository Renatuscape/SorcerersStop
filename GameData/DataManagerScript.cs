using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManagerScript : MonoBehaviour
{
    public string version;
    public string lastVersionSaved;

    //PLAYER DATA
    public string playerName;
    public string pronounSub;
    public string pronounObj;
    public string pronounGen;

    public string playerNameColour;
    public int playerGold;

    public List<IdIntPair> inventoryList = Player.inventoryList;
    public List<IdIntPair> questProgression = Player.questProgression;

    //GAME SETTINGS
    public float autoPlaySpeed;
    public bool alwaysHideCoachExterior;

    //PLAYER SPRITE
    public int hairIndex;
    public int bodyIndex;
    public int headIndex;
    public int eyesIndex;
    public int mouthIndex;

    public string hairHexColour;
    public string eyesHexColour;
    public string frameID;

    //JOURNEY DATA - SAVE READY
    public string postLocationID;
    public string currentRegion;
    public float mapPositionX;
    public float mapPositionY;
    public float timeOfDay;
    public int totalGameDays;
    public List<string> giftedThisWeek;
    public List<string> unlockedNames;

    //PASSENGER DATA - A
    public bool passengerIsActiveA;
    public string passengerNameA;
    public Sprite passengerSpriteA;
    public Location passengerOriginA;
    public Location passengerDestinationA;
    public List<string> passengerChatListA;

    //PASSENGER DATA - B
    public bool passengerIsActiveB;
    public string passengerNameB;
    public Sprite passengerSpriteB;
    public Location passengerOriginB;
    public Location passengerDestinationB;
    public List<string> passengerChatListB;

    //PLANTER - A
    public bool planterIsActiveA;
    public int planterSpriteA;
    public string seedA;
    public float progressSeedA;
    public int seedHealthA;

    //PLANTER - B
    public bool planterIsActiveB;
    public int planterSpriteB;
    public string seedB;
    public float progressSeedB;
    public int seedHealthB;

    //PLANTER - C
    public bool planterIsActiveC;
    public int planterSpriteC;
    public string seedC;
    public float progressSeedC;
    public int seedHealthC;

    //ALCHEMY SYNTHESISERS
    public List<SynthesiserData> alchemySynthesisers;
}

[Serializable]
public class SynthesiserData
{
    public string synthesiserID;
    public bool isSynthActive;
    public Recipe synthRecipe;
    public float progressSynth;
    public bool isSynthPaused;
    public bool consumesMana;
}

[Serializable]
public class PlanterData
{
    public string planterID;
    public bool planterIsActive;
    public int planterSprite;
    public string seed;
    public float progressSeed;
    public int seedHealth;
}