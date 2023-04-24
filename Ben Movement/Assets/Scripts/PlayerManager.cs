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

    private List<Player> m_Players = new List<Player>();
    public List<Player> players
    {
        get
        {
            return m_Players;
        }
        set
        {
            if (m_Players == value)
                return;
            m_Players = value;
            if(OnPlayerListChange != null)
                OnPlayerListChange();
        }
    }

    [Header("Input")]
    PlayerInput playerInput;
    [SerializeField] InputActionAsset actionAsset;
    InputSystemUIInputModule inputModule;
    string playerControlScheme;
    InputDevice[] devices;

    [Header("Rendering")]
    [SerializeField] Material[] playerMaterials;
    [SerializeField] RenderTexture normalRenderTexture;
    [SerializeField] RenderTexture[] tallRenderTextures;
    [SerializeField] RenderTexture[] smallRenderTextures;
    ScreenManager screenManager;

    [Header("UI")]
    bool isMenuOpen;

    [Header("Camera Sizes")]
    [SerializeField] float largeCameraSize;
    [SerializeField] float tallCameraSize;
    [SerializeField] float smallCameraSize;

    public bool IsMenuOpen
    {
        get
        {
            return isMenuOpen;
        }

        set
        {
            if (value == true)
            {
                PauseGame();
                isMenuOpen = value;
            }
            else
            {
                UnpauseGame();
                isMenuOpen = value;
            }
        }
    }

    public bool isPaused;

    private void Start()
    {
        screenManager = FindObjectOfType<ScreenManager>();
    }

    void PauseGame()
    {
        Time.timeScale = 0f;
        isPaused = true;
    }

    void UnpauseGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
    }

    Vector3 GetPlayerSpawnPoint()
    {
        return new Vector3(Random.Range(-2f, 2f), 0.5f, Random.Range(-2f, 2f));
    }

    public void PlayerSpawned(Player player)
    {
        //find spawn position
        player.playerMovement.transform.position = GetPlayerSpawnPoint();


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
        inputModule.move = InputActionReference.Create(actionMap.FindAction("Menu Move", true));
        inputModule.submit = InputActionReference.Create(actionMap.FindAction("Menu Select", true));



        //re-apply correct control scheme
        playerInput.SwitchCurrentControlScheme(playerControlScheme, devices);

        //update rebinding actions 
        RebindActionUI[] rebindingActions = player.GetComponentsInChildren<RebindActionUI>(true);
        foreach(RebindActionUI binding in rebindingActions)
        {
            string actionName = binding.actionReference.action.name;
            string oldMapName = binding.actionReference.action.actionMap.name;

            InputActionMap newMap = newActionAsset.FindActionMap(oldMapName, true);
            InputActionReference newReference = InputActionReference.Create(newMap.FindAction(actionName, true));

            binding.actionReference = newReference;

            Debug.Log(newReference.ToString());
        }

        //re-apply in scrolling lists
        ScrollRectAutoScroll[] scrollers = player.GetComponentsInChildren<ScrollRectAutoScroll>(true);
        foreach (ScrollRectAutoScroll scroller in scrollers)
        {
            scroller.eventSystem = player.GetComponentInChildren<MultiplayerEventSystem>();
        }


        playerCount++;
        StartCoroutine(screenManager.PlayerJoined(playerCount));
        player.GetComponentInChildren<PlayerController>().playerVisuals.GetComponentInChildren<Renderer>().material = playerMaterials[player.playerNumber - 1];

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
                players[0].GetComponentInChildren<Camera>().orthographicSize = largeCameraSize;
                break;
            case 2:
                players[0].GetComponentInChildren<Camera>().targetTexture = tallRenderTextures[0];
                players[0].GetComponentInChildren<Camera>().orthographicSize = tallCameraSize;
                players[1].GetComponentInChildren<Camera>().targetTexture = tallRenderTextures[1];
                players[1].GetComponentInChildren<Camera>().orthographicSize = tallCameraSize;
                break;
            case 3:
                players[0].GetComponentInChildren<Camera>().targetTexture = smallRenderTextures[0];
                players[0].GetComponentInChildren<Camera>().orthographicSize = smallCameraSize;
                players[1].GetComponentInChildren<Camera>().targetTexture = tallRenderTextures[1];
                players[1].GetComponentInChildren<Camera>().orthographicSize = tallCameraSize;
                players[2].GetComponentInChildren<Camera>().targetTexture = smallRenderTextures[2];
                players[2].GetComponentInChildren<Camera>().orthographicSize = smallCameraSize;
                break;
            case 4:
                players[0].GetComponentInChildren<Camera>().targetTexture = smallRenderTextures[0];
                players[0].GetComponentInChildren<Camera>().orthographicSize = smallCameraSize;
                players[1].GetComponentInChildren<Camera>().targetTexture = smallRenderTextures[1];
                players[1].GetComponentInChildren<Camera>().orthographicSize = smallCameraSize;
                players[2].GetComponentInChildren<Camera>().targetTexture = smallRenderTextures[2];
                players[2].GetComponentInChildren<Camera>().orthographicSize = smallCameraSize;
                players[3].GetComponentInChildren<Camera>().targetTexture = smallRenderTextures[3];
                players[3].GetComponentInChildren<Camera>().orthographicSize = smallCameraSize;
                break;
        }
    }

    //delegate for AI that needs accurate list of players.
    public delegate void OnPlayerListChangeDelegate();
    public event OnPlayerListChangeDelegate OnPlayerListChange;





}
