using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public DataManagerScript dataManager;
    public Toggle toggleCoachExterior;
    public Toggle toggleTrueName;
    public Toggle toggleRandomiseOnDeath;
    public Toggle toggleCustomisationOnDeath;
    public Toggle toggleForceSaveOnDeath;

    public TMP_Dropdown resolutionDropdown;
    public GameObject content;
    public GameObject behaviourPanel;

    void Start()
    {
        toggleCoachExterior.onValueChanged.AddListener(ToggleAlwaysHideCoachExterior);
        toggleTrueName.onValueChanged.AddListener(ToggleTrueName);

        resolutionDropdown.onValueChanged.AddListener(ResolutionDropDown);

        content.SetActive(false);
    }

    public void Open()
    {
        if (TransientDataScript.GameState == GameState.MainMenu)
        {
            behaviourPanel.SetActive(false);
        }
        else
        {
            behaviourPanel.SetActive(true);
        }

        toggleCoachExterior.isOn = GlobalSettings.AlwaysHideCoachExterior;
        toggleTrueName.isOn = GlobalSettings.AlwaysTrueNamePlate;

        if (dataManager != null)
        {
            toggleRandomiseOnDeath.isOn = dataManager.randomiseOnDeath;
            toggleCustomisationOnDeath.isOn = dataManager.disableCustomisationOnDeath;
            toggleForceSaveOnDeath.isOn = dataManager.saveOnDeath;
        }

        content.SetActive(true);
    }

    public void Close()
    {
        content.SetActive(false);
    }


    void OnEnable()
    {
        toggleCoachExterior.isOn = GlobalSettings.AlwaysHideCoachExterior;
    }

    void ToggleAlwaysHideCoachExterior(bool toggle)
    {
        GlobalSettings.AlwaysHideCoachExterior = toggle;
        GlobalSettings.SaveSettings();
    }

    void ToggleTrueName(bool toggle)
    {
        GlobalSettings.AlwaysTrueNamePlate = toggle;
        GlobalSettings.SaveSettings();
    }

    void ResolutionDropDown(int option)
    {
        switch (option)
        {
            case 0:
                Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
                break;
            case 1:
                Screen.SetResolution(1920, 1080, false);
                break;
            case 2:

                Screen.SetResolution(1440, 810, false);
                break;
        }

        Debug.Log(Screen.currentResolution);
    }
}