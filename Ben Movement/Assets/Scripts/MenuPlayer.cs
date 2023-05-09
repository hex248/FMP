using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuPlayer : MonoBehaviour
{
    public int playerIndex;
    public PlayerInput playerInput;
    PlayerMenuManager menuManager;
    bool isReady;

    public InputDevice[] devices;
    public string playerControlScheme;

    void SetUp()
    {
        Debug.Log("Player Placeholder");
        playerInput = GetComponentInChildren<PlayerInput>();
        playerIndex = playerInput.playerIndex + 1;
        menuManager = FindObjectOfType<PlayerMenuManager>();
        menuManager.PlayerJoined(this);
        isReady = false;

       
    }

    void Start()
    {
        SetUp();
    }

    public void ReadyUp(InputAction.CallbackContext context)
    {
        if (!isReady && context.performed)
        {
            menuManager.PlayerReady(this);
            isReady = true;
        }
    }


}
