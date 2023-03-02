using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    ScreenManager screenManager;
    PlayerManager playerManager;
    PlayerInput playerInput;
    PlayerInputManager playerInputManager;
    PlayerMovement playerMovement;

    [Header("Player Details")]
    public int playerNumber;

    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponentInChildren<PlayerInput>();
        playerManager = FindObjectOfType<PlayerManager>();
        playerMovement = GetComponentInChildren<PlayerMovement>();

        playerInputManager = FindObjectOfType<PlayerInputManager>();
        playerNumber = playerInput.playerIndex + 1;
        playerManager.PlayerSpawned(this);
        screenManager = FindObjectOfType<ScreenManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
