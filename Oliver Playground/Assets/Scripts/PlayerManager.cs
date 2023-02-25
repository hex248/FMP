using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerManager : MonoBehaviour
{
    [Header("Players")]
    public int playerCount = 0;

    public List<PlayerMovement> players = new List<PlayerMovement>();

    [Header("Rendering")]
    [SerializeField] Material[] playerMaterials;
    [SerializeField] RenderTexture[] normalRenderTextures;
    [SerializeField] RenderTexture[] tallRenderTextures;
    ScreenManager screenManager;

    private void Start()
    {
        screenManager = FindObjectOfType<ScreenManager>();
    }

    public void PlayerSpawned(PlayerMovement player)
    {
        players.Add(player);
        playerCount++;
        StartCoroutine(screenManager.PlayerJoined(playerCount));
        player.mainModel.GetComponentInChildren<Renderer>().material = playerMaterials[player.playerNumber - 1];

        UpdateRenderTextures();
    }

    public void UpdateRenderTextures()
    {
        switch (playerCount)
        {
            case 1:
                players[0].camera.targetTexture = normalRenderTextures[0];
                break;
            case 2:
                players[0].camera.targetTexture = tallRenderTextures[0];
                players[1].camera.targetTexture = tallRenderTextures[1];
                break;
            case 3:
                players[0].camera.targetTexture = normalRenderTextures[0];
                players[1].camera.targetTexture = tallRenderTextures[1];
                players[2].camera.targetTexture = normalRenderTextures[2];
                break;
            case 4:
                players[0].camera.targetTexture = normalRenderTextures[0];
                players[1].camera.targetTexture = normalRenderTextures[1];
                players[2].camera.targetTexture = normalRenderTextures[2];
                players[3].camera.targetTexture = normalRenderTextures[3];
                break;
        }
    }
}
