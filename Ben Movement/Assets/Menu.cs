using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Menu : MonoBehaviour
{
    enum MenuPage {MainMenu, SettingsMenu, ControlsMenu, None}
    MenuPage currentPage;

    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject mainMenuFirstSelected;
    [SerializeField] GameObject settingsMenu;
    [SerializeField] GameObject settingsFirstSelected;
    [SerializeField] GameObject controlsMenu;
    [SerializeField] GameObject controlsFirstSelected;

    bool menuIsOpen;

    public void OpenMainMenu()
    {
        menuIsOpen = true;
        mainMenu.SetActive(true);
        settingsMenu.SetActive(false);
        controlsMenu.SetActive(false);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(mainMenuFirstSelected);
    }

    public void CloseMenu()
    {
        menuIsOpen = false;
        mainMenu.SetActive(false);
        settingsMenu.SetActive(false);
        controlsMenu.SetActive(false);

        EventSystem.current.SetSelectedGameObject(null);
    }

    public void OpenSettingsMenu()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
        controlsMenu.SetActive(false);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(settingsFirstSelected);
    }

    public void OpenControlsMenu()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(false);
        controlsMenu.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(controlsFirstSelected);
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

    public void GoToPrevious()
    {
        switch(currentPage)
        {
            case MenuPage.MainMenu:
                CloseMenu();
                break;
            case MenuPage.SettingsMenu:
                OpenMainMenu();
                break;
            case MenuPage.ControlsMenu:
                OpenSettingsMenu();
                break;
            case MenuPage.None:
                break;
            default:
                break;

        }
    }

    void Start()
    {
        CloseMenu();
    }

}
