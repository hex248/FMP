using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;


public class PlayerManager : MonoBehaviour
{
    [Header("Players")]
    public int playerCount = 0;

    public List<Player> players = new List<Player>();

    [Header("Input")]
    [SerializeField] InputActionAsset actionAsset;
    InputSystemUIInputModule inputModule;
    [SerializeField] InputActionReference menuMove;

    [Header("Rendering")]
    [SerializeField] Material[] playerMaterials;
    [SerializeField] RenderTexture normalRenderTexture;
    [SerializeField] RenderTexture[] tallRenderTextures;
    [SerializeField] RenderTexture[] smallRenderTextures;
    ScreenManager screenManager;

    [Header("UI")]
    public bool isMenuOpen;

    private void Start()
    {
        screenManager = FindObjectOfType<ScreenManager>();
    }

    public void PlayerSpawned(Player player)
    {
        players.Add(player);

        InputActionAsset newActionAsset = Instantiate(actionAsset);
        player.GetComponentInChildren<PlayerInput>().actions = newActionAsset;

        inputModule = player.GetComponentInChildren<InputSystemUIInputModule>();
        inputModule.actionsAsset = newActionAsset;
        InputActionMap actionMap = newActionAsset.FindActionMap("Player", true);

        //have to rebind UI controls
        Debug.Log(InputActionReference.Create(actionMap.FindAction("Menu Move", true)));
        menuMove = InputActionReference.Create(actionMap.FindAction("Menu Move", true));
        inputModule.move = InputActionReference.Create(actionMap.FindAction("Menu Move", true));
        inputModule.submit = InputActionReference.Create(actionMap.FindAction("Menu Select", true));

        playerCount++;
        StartCoroutine(screenManager.PlayerJoined(playerCount));
        player.GetComponentInChildren<PlayerMovement>().playerVisuals.GetComponentInChildren<Renderer>().material = playerMaterials[player.playerNumber - 1];

        UpdateRenderTextures();
    }

    public void DisconnectPlayer(Player player)
    {
        players.RemoveAt(player.playerNumber - 1);
        playerCount--;
    }

    public void UpdateRenderTextures()
    {
        switch (playerCount)
        {
            case 1:
                players[0].GetComponentInChildren<Camera>().targetTexture = normalRenderTexture;
                break;
            case 2:
                players[0].GetComponentInChildren<Camera>().targetTexture = tallRenderTextures[0];
                players[1].GetComponentInChildren<Camera>().targetTexture = tallRenderTextures[1];
                break;
            case 3:
                players[0].GetComponentInChildren<Camera>().targetTexture = smallRenderTextures[0];
                players[1].GetComponentInChildren<Camera>().targetTexture = tallRenderTextures[1];
                players[2].GetComponentInChildren<Camera>().targetTexture = smallRenderTextures[2];
                break;
            case 4:
                players[0].GetComponentInChildren<Camera>().targetTexture = smallRenderTextures[0];
                players[1].GetComponentInChildren<Camera>().targetTexture = smallRenderTextures[1];
                players[2].GetComponentInChildren<Camera>().targetTexture = smallRenderTextures[2];
                players[3].GetComponentInChildren<Camera>().targetTexture = smallRenderTextures[3];
                break;
        }
    }

    
}
