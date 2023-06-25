using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.Samples.RebindUI;
using UnityEngine.SceneManagement;


public class PlayerManager : MonoBehaviour
{
    public GameObject endGameTimeline;

    [Header("Players")]
    public int playerCount = 0;
    [SerializeField] GameObject playerParentPrefab;
    private List<Player> m_Players = new List<Player>();
    public List<Player> players = new List<Player>();

    [Header("Input")]
    PlayerInput playerInput;
    [SerializeField] InputActionAsset actionAsset;
    InputSystemUIInputModule inputModule;
    string playerControlScheme;
    InputDevice[] devices;

    [Header("Rendering")]
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

    [Header("Health Bars")]
    [SerializeField] PlayerHealthBar[] healthBars1Player;
    [SerializeField] PlayerHealthBar[] healthBars2Player;
    [SerializeField] PlayerHealthBar[] healthBars3Player;
    [SerializeField] PlayerHealthBar[] healthBars4Player;

    [Header("Materials")]
    [SerializeField] Material[] matP1;
    [SerializeField] Material[] matP2;
    [SerializeField] Material[] matP3;
    [SerializeField] Material[] matP4;

    [Header("Eye Materials")]
    [SerializeField] Material eyeMatP1;
    [SerializeField] Material eyeMatP2;
    [SerializeField] Material eyeMatP3;
    [SerializeField] Material eyeMatP4;

    [Header("Projectile Materials")]
    [SerializeField] Material projectileMatP1;
    [SerializeField] Material projectileMatP2;
    [SerializeField] Material projectileMatP3;
    [SerializeField] Material projectileMatP4;

    [Header("Trail Materials")]
    [SerializeField] Material trailMatP1;
    [SerializeField] Material trailMatP2;
    [SerializeField] Material trailMatP3;
    [SerializeField] Material trailMatP4;

    [Header("Particle Effects")]
    [SerializeField] GameObject particleP1;
    [SerializeField] GameObject particleP2;
    [SerializeField] GameObject particleP3;
    [SerializeField] GameObject particleP4;

    [Header("Game Settings")]
    [SerializeField] int playersRemaining;


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
        playersRemaining = 0;
        endGameTimeline.SetActive(false);
        AddJoinedPlayers();
    }

    private void Awake()
    {
        OnPlayerListChange += DummyListener;
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

    Vector3 GetPlayerSpawnPoint(int index)
    {
        float angle = -Mathf.PI + index * Mathf.PI * 0.5f;
        float x = Mathf.Sin(angle) * 6f;
        float z = Mathf.Cos(angle) * 6f;
        Vector3 spawnPosition = new Vector3(x, 0.5f, z);
        return spawnPosition;
    }

    void AddJoinedPlayers()
    {
        int joinedPlayerCount = PlayerSetupInfo.playerCount;
        
        if(joinedPlayerCount >= 1)
        {
            GameObject player = Instantiate(playerParentPrefab);
            player.GetComponentInChildren<PlayerInput>().SwitchCurrentControlScheme(PlayerSetupInfo.player1Devices); 

        }
        if(joinedPlayerCount >= 2)
        {
            GameObject player = Instantiate(playerParentPrefab);
            player.GetComponentInChildren<PlayerInput>().SwitchCurrentControlScheme(PlayerSetupInfo.player2Devices);
        }
        if (joinedPlayerCount >= 3)
        {
            GameObject player = Instantiate(playerParentPrefab);
            player.GetComponentInChildren<PlayerInput>().SwitchCurrentControlScheme(PlayerSetupInfo.player3Devices);
        }
        if (joinedPlayerCount == 4)
        {
            GameObject player = Instantiate(playerParentPrefab);
            player.GetComponentInChildren<PlayerInput>().SwitchCurrentControlScheme(PlayerSetupInfo.player4Devices);
        }
    }

    public void PlayerSpawned(Player player)
    {
        //find spawn position
        player.playerMovement.transform.position = GetPlayerSpawnPoint(player.playerNumber);
        
        switch (player.playerNumber)
        {   
            case 2:
                player.ChangeMaterials(matP1);
                player.ChangeEyes(eyeMatP1);
                player.SetProjectileMat(projectileMatP1);
                player.SetTrailMaterial(trailMatP1);
                player.SetParticles(particleP1);
                player.SetHat(0);
                break;
            case 3:
                player.ChangeMaterials(matP2);
                player.ChangeEyes(eyeMatP2);
                player.SetProjectileMat(projectileMatP2);
                player.SetTrailMaterial(trailMatP2);
                player.SetParticles(particleP2);
                player.SetHat(1);
                break;
            case 4:
                player.ChangeMaterials(matP3);
                player.ChangeEyes(eyeMatP3);
                player.SetProjectileMat(projectileMatP3);
                player.SetTrailMaterial(trailMatP3);
                player.SetParticles(particleP3);
                player.SetHat(2);
                break;
            case 1:
                player.ChangeMaterials(matP4);
                player.ChangeEyes(eyeMatP4);
                player.SetProjectileMat(projectileMatP4);
                player.SetTrailMaterial(trailMatP4);
                player.SetParticles(particleP4);
                player.SetHat(3);
                break;
            default:
                player.ChangeMaterials(matP1);
                player.ChangeEyes(eyeMatP1);
                player.SetProjectileMat(projectileMatP1);
                player.SetTrailMaterial(trailMatP1);
                player.SetParticles(particleP1);
                player.SetHat(0);
                break;
        }
        
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

        }

        //re-apply in scrolling lists
        ScrollRectAutoScroll[] scrollers = player.GetComponentsInChildren<ScrollRectAutoScroll>(true);
        foreach (ScrollRectAutoScroll scroller in scrollers)
        {
            scroller.eventSystem = player.GetComponentInChildren<MultiplayerEventSystem>();
        }



        playerCount++;
        playersRemaining++;
        OnPlayerListChange();

        
        StartCoroutine(screenManager.PlayerJoined(playerCount));


        UpdateRenderTextures();
        AssignPlayerHealthBars();

    }

    void AssignPlayerHealthBars()
    {

        for(int i = 0; i < playerCount; i++)
        {
            Player player = players[i];
            if (players.Count == 1)
            {
                player.playerHealthBar = healthBars1Player[i];
            }
            else if (players.Count == 2)
            {
                player.playerHealthBar = healthBars2Player[i];
            }
            else if (players.Count == 3)
            {
                player.playerHealthBar = healthBars3Player[i];
            }
            else if (players.Count == 4)
            {
                player.playerHealthBar = healthBars4Player[i];
            }

        }
    }

    public void DisconnectPlayer(Player player)
    {
        players.RemoveAt(player.playerNumber - 1);
        OnPlayerListChange();
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

    void DummyListener()
    {
        //dummy
    }

    public void PlayerDied()
    {
        playersRemaining--;
        if(playersRemaining <= 0 || FindObjectOfType<Bed>().GetCurrentHealth() <= 0.0f || FindObjectOfType<WaveManager>().bossRound)
        {
            endGameTimeline.SetActive(true);
        }
    }

    public void RevivePlayers()
    {
        foreach(Player player in players)
        {
            if (player.playerHealth.IsDead())
            {
                playersRemaining++;
                player.playerHealth.Revive();
            }
        }
            
    }

    public void EndGame(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        //end game logic
        //Application.Quit();
    }

}
