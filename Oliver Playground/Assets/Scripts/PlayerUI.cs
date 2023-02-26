using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum Menu
{
    none,
    pause,
    settings
}

public class PlayerUI : MonoBehaviour
{
    public bool paused = false;
    public GameObject pauseScreen;
    public GameObject settingsScreen;

    public Menu currentMenu = Menu.none;

    private void Update()
    {
        if (paused && currentMenu == Menu.none) currentMenu = Menu.pause;
        if (!paused) currentMenu = Menu.none;
        pauseScreen.SetActive(currentMenu == Menu.pause);
        settingsScreen.SetActive(currentMenu == Menu.settings);
    }

    public void OnPauseResume(InputAction.CallbackContext context)
    {
        paused = !paused;
    }

    public void OnPauseResume()
    {
        paused = !paused;
    }

    public void OnBack()
    {
        if (currentMenu == Menu.pause) OnPauseResume();
        else if (currentMenu == Menu.settings) currentMenu = Menu.pause;
    }

    public void Settings()
    {
        currentMenu = Menu.settings;
    }
}
