using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class Menu : MonoBehaviour
{
    enum MenuPage {MainMenu, SettingsMenu, ControlsMenu, AccessibilityMenu, None}
    MenuPage currentPage;

    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject mainMenuFirstSelected;
    [SerializeField] GameObject settingsMenu;
    [SerializeField] GameObject settingsFirstSelected;
    [SerializeField] GameObject controlsMenu;
    [SerializeField] GameObject controlsFirstSelected;
    [SerializeField] GameObject accessibiltyMenu;
    [SerializeField] GameObject accessibilityFirstSelected;

    bool menuIsOpen;

    public void OpenMainMenu()
    {
        menuIsOpen = true;
        currentPage = MenuPage.MainMenu;

        mainMenu.SetActive(true);
        settingsMenu.SetActive(false);
        controlsMenu.SetActive(false);
        controlsMenu.SetActive(false);
        accessibiltyMenu.SetActive(false);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(mainMenuFirstSelected);
    }

    public void OpenAccessiblityMenu()
    {
        currentPage = MenuPage.AccessibilityMenu;

        mainMenu.SetActive(false);
        settingsMenu.SetActive(false);
        controlsMenu.SetActive(false);
        accessibiltyMenu.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(accessibilityFirstSelected);
    }

    public void CloseMenu()
    {
        menuIsOpen = false;
        currentPage = MenuPage.None;

        mainMenu.SetActive(false);
        settingsMenu.SetActive(false);
        controlsMenu.SetActive(false);
        accessibiltyMenu.SetActive(false);

        EventSystem.current.SetSelectedGameObject(null);
    }

    public void OpenSettingsMenu()
    {
        currentPage = MenuPage.SettingsMenu;

        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
        controlsMenu.SetActive(false);
        accessibiltyMenu.SetActive(false);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(settingsFirstSelected);
    }

    public void OpenControlsMenu()
    {
        currentPage = MenuPage.ControlsMenu;

        mainMenu.SetActive(false);
        settingsMenu.SetActive(false);
        controlsMenu.SetActive(true);
        accessibiltyMenu.SetActive(false);

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
            case MenuPage.AccessibilityMenu:
                OpenSettingsMenu();
                break;
            case MenuPage.None:
                break;
            default:
                break;

        }
    }

    public void ReturnKeyPressed(InputAction.CallbackContext context)
    {
        bool triggered = context.action.triggered;
        if (triggered)
        {
            if(menuIsOpen)
            {
                GoToPrevious();
            }
        }
    }

    public void Test()
    {
        Debug.Log(currentPage);
    }

    void Start()
    {
        CloseMenu();
    }
}
