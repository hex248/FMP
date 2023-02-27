using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject settingsMenu;
    [SerializeField] GameObject controlsMenu;
    bool menuIsOpen;

    public void OpenMainMenu()
    {
        menuIsOpen = true;
        mainMenu.SetActive(true);
        settingsMenu.SetActive(false);
        controlsMenu.SetActive(false);
    }

    public void CloseMenu()
    {
        menuIsOpen = false;
        mainMenu.SetActive(false);
        settingsMenu.SetActive(false);
        controlsMenu.SetActive(false);
    }

    public void OpenSettingsMenu()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
        controlsMenu.SetActive(false);
    }

    public void OpenControlsMenu()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(false);
        controlsMenu.SetActive(true);
    }

    public void ToggleMenu()
    {
        if(menuIsOpen)
        {
            CloseMenu();
        }
        else
        {
            OpenMainMenu();
        }
    }

    void Start()
    {
        CloseMenu();
    }

}
