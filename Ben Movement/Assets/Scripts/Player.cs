using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    ScreenManager screenManager;
    PlayerManager playerManager;
    public PlayerInput playerInput;
    PlayerInputManager playerInputManager;
    public PlayerController playerMovement;
    ColorblindFilters filterController;
    public CameraFollow cameraFollow;
    public Camera playerCamera;

    [Header("Player Details")]
    public int playerNumber;
    public PlayerHealthBar playerHealthBar;
    public PlayerHealth playerHealth;

    // Start is called before the first frame update
    void Awake()
    {
        playerInput = GetComponentInChildren<PlayerInput>();
        playerManager = FindObjectOfType<PlayerManager>();
        playerMovement = GetComponentInChildren<PlayerController>();

        playerInputManager = FindObjectOfType<PlayerInputManager>();
        playerNumber = playerInput.playerIndex + 1;
        playerManager.PlayerSpawned(this);
        screenManager = FindObjectOfType<ScreenManager>();

        cameraFollow = GetComponentInChildren<CameraFollow>();
        playerCamera = cameraFollow.GetComponentInChildren<Camera>();
        playerHealth = GetComponentInChildren<PlayerHealth>();
    }

    // Update is called once per frame
    void Update()
    {

    }

}
