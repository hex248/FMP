using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using TMPro;

public class Menu : MonoBehaviour
{
    enum MenuPage {MainMenu, SettingsMenu, ControlsMenu, AccessibilityMenu, None}
    MenuPage currentPage;
    [SerializeField] EventSystem eventSystem;

    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject mainMenuFirstSelected;
    [SerializeField] GameObject settingsMenu;
    [SerializeField] GameObject settingsFirstSelected;
    [SerializeField] GameObject controlsMenu;
    [SerializeField] GameObject controlsFirstSelected;
    [SerializeField] GameObject accessibiltyMenu;
    [SerializeField] GameObject accessibilityFirstSelected;

    PlayerManager playerManager;

    bool isMenuOpen;

    void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        CloseMenu();
    }

    public void OpenMainMenu()
    {
        isMenuOpen = true;
        playerManager.isMenuOpen = true;
        currentPage = MenuPage.MainMenu;

        mainMenu.SetActive(true);
        settingsMenu.SetActive(false);
        controlsMenu.SetActive(false);
        controlsMenu.SetActive(false);
        accessibiltyMenu.SetActive(false);

        eventSystem.SetSelectedGameObject(null);
        eventSystem.SetSelectedGameObject(mainMenuFirstSelected);
    }

    public void OpenAccessiblityMenu()
    {
        currentPage = MenuPage.AccessibilityMenu;

        mainMenu.SetActive(false);
        settingsMenu.SetActive(false);
        controlsMenu.SetActive(false);
        accessibiltyMenu.SetActive(true);

        eventSystem.SetSelectedGameObject(null);
        eventSystem.SetSelectedGameObject(accessibilityFirstSelected);
    }

    public void CloseMenu()
    {
        isMenuOpen = false;
        playerManager.isMenuOpen = false;
        Debug.Log("close menu");

        currentPage = MenuPage.None;

        mainMenu.SetActive(false);
        settingsMenu.SetActive(false);
        controlsMenu.SetActive(false);
        accessibiltyMenu.SetActive(false);

        eventSystem.SetSelectedGameObject(null);
    }

    public void OpenSettingsMenu()
    {
        currentPage = MenuPage.SettingsMenu;

        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
        controlsMenu.SetActive(false);
        accessibiltyMenu.SetActive(false);

        eventSystem.SetSelectedGameObject(null);
        eventSystem.SetSelectedGameObject(settingsFirstSelected);
    }

    public void OpenControlsMenu()
    {
        currentPage = MenuPage.ControlsMenu;

        mainMenu.SetActive(false);
        settingsMenu.SetActive(false);
        controlsMenu.SetActive(true);
        accessibiltyMenu.SetActive(false);

        eventSystem.SetSelectedGameObject(null);
        eventSystem.SetSelectedGameObject(controlsFirstSelected);
    }

    public void ToggleMenu()
    {
        if(isMenuOpen)
        {
            CloseMenu();
        }
        else
        {
            if(!playerManager.isMenuOpen)
            {
               OpenMainMenu();
            }
            
        }
    }

    public void MenuReturn()
    {
        if(isMenuOpen)
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
    }

    public void ReturnButtonPressed(InputAction.CallbackContext context)
    {
        bool triggered = context.action.triggered;
        if (triggered)
        {
            MenuReturn();
        }
    }

    public void ToggleButtonPressed(InputAction.CallbackContext context)
    {
        bool triggered = context.action.triggered;
        if (triggered)
        {
            ToggleMenu();
        }

    }

    public void Test()
    {
        Debug.Log(currentPage);
    }


    public void ChangeControlScheme()
    {
        
    }

    

    
}
