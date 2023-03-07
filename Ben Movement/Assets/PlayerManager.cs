using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.Samples.RebindUI;


public class PlayerManager : MonoBehaviour
{
    [Header("Players")]
    public int playerCount = 0;

    public List<Player> players = new List<Player>();

    [Header("Input")]
    PlayerInput playerInput;
    [SerializeField] InputActionAsset actionAsset;
    InputSystemUIInputModule inputModule;
    [SerializeField] InputActionReference menuMove;
    string playerControlScheme;
    InputDevice[] devices;

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
        playerInput = player.GetComponentInChildren<PlayerInput>();

        //save important info before it's overidden by changing the asset
        playerControlScheme = playerInput.currentControlScheme;
        devices = playerInput.devices.ToArray();


        InputActionAsset newActionAsset = Instantiate(actionAsset);

        inputModule = player.GetComponentInChildren<InputSystemUIInputModule>();

        //SWAPPING THESE 2 LINES FOR SOME REASON MEANS THE UI STAYS UNBOUND - IT APPEARS UNBOUND IN THE INSPECTOR ANYWAY
        inputModule.actionsAsset = newActionAsset;
        playerInput.actions = newActionAsset;

        InputActionMap actionMap = newActionAsset.FindActionMap("Player", true);
        menuMove = InputActionReference.Create(actionMap.FindAction("Menu Move", true));
        inputModule.move = InputActionReference.Create(actionMap.FindAction("Menu Move", true));
        inputModule.submit = InputActionReference.Create(actionMap.FindAction("Menu Select", true));



        //re-apply correct control scheme
        playerInput.SwitchCurrentControlScheme(playerControlScheme, devices);

        //update rebinding actions 
        RebindActionUI[] rebindingActions = player.GetComponentsInChildren<RebindActionUI>();
        foreach(RebindActionUI binding in rebindingActions)
        {
            string actionName = binding.actionReference.action.name;
            string oldMapName = binding.actionReference.action.actionMap.name;

            InputActionMap newMap = newActionAsset.FindActionMap(oldMapName, true);
            InputActionReference newReference = InputActionReference.Create(newMap.FindAction(actionName, true));

            binding.actionReference = newReference;

            Debug.Log(newReference.ToString());
        }


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
