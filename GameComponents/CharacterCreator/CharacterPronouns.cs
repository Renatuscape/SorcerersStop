using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterPronouns : MonoBehaviour
{
    public DataManagerScript dataManager;
    public TMP_Dropdown pronounDropdown;
    public TMP_Dropdown genderDropdown;
    public GameObject customFields;

    public TextMeshProUGUI pronounSub;
    public TextMeshProUGUI pronounObj;
    public TextMeshProUGUI pronounGen;
    public TextMeshProUGUI genderText;
    private void Awake()
    {
        customFields.SetActive(false);
        dataManager.pronounSub = pronounSub.text;
        dataManager.pronounObj = pronounObj.text;
        dataManager.pronounGen = pronounGen.text;
    }

    public void ChoosePronouns()
    {
        if (pronounDropdown.value != 4)
        {
            customFields.SetActive(false);
        }

        if (pronounDropdown.value > 5 || pronounDropdown.value < 0)
            pronounDropdown.value = 0;

        if (pronounDropdown.value == 0)
        {
            pronounSub.text = "he";
            pronounObj.text = "him";
            pronounGen.text = "his";
        }

        else if (pronounDropdown.value == 1)
        {
            pronounSub.text = "she";
            pronounObj.text = "her";
            pronounGen.text = "her";
        }
        else if (pronounDropdown.value == 2)
        {
            pronounSub.text = "they";
            pronounObj.text = "them";
            pronounGen.text = "their";
        }
        else if (pronounDropdown.value == 3)
        {
            pronounSub.text = "it";
            pronounObj.text = "it";
            pronounGen.text = "its";
        }
        else if (pronounDropdown.value == 4)
        {
            customFields.SetActive(true);
        }

        ChooseGender(pronounDropdown.value);
        UpdatePronouns();
    }

    public void ChooseGender(int choice = -1)
    {
        if (choice != -1)
        {
            choice = genderDropdown.value;
        }
        if (choice == 0)
        {
            dataManager.playerGender = "GENDERMALE";
            genderText.text = "Male";
        }
        else if (choice == 1)
        {
            dataManager.playerGender = "GENDERFEMALE";
            genderText.text = "Female";
        }
        else if (choice >= 2)
        {
            dataManager.playerGender = "GENDEROTHER";
            genderText.text = "Other";
        }
    }

    public void UpdatePronouns()
    {
        dataManager.pronounSub = pronounSub.text;
        dataManager.pronounObj = pronounObj.text;
        dataManager.pronounGen = pronounGen.text;
        StartCoroutine(LatePronounUpdate());
    }

    IEnumerator LatePronounUpdate()
    {
        yield return new WaitForSeconds(0.1f);
        dataManager.pronounSub = pronounSub.text;
        dataManager.pronounObj = pronounObj.text;
        dataManager.pronounGen = pronounGen.text;
    }
}
