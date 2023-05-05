using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuPlayer : MonoBehaviour
{
    public int playerIndex;
    public PlayerInput playerInput;
    PlayerMenuManager menuManager;

    void SetUp()
    {
        Debug.Log("Player Placeholder");
        playerInput = GetComponentInChildren<PlayerInput>();
        playerIndex = playerInput.playerIndex + 1;
        menuManager = FindObjectOfType<PlayerMenuManager>();
        menuManager.PlayerJoined(this);
    }

    void Start()
    {
        SetUp();
    }
}
