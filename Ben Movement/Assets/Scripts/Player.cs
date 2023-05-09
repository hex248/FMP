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

    public Transform particlePosition;

    [Header("Renderers to Change")]
    public Renderer[] meshRenderer;
    public GameObject[] hats;
    public Renderer[] eyes;
    public GameObject[] trails;

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

    public void SetHat(int index)
    {
        foreach(GameObject g in hats)
        {
            g.SetActive(false);
        }
        hats[index].SetActive(true);
    }

    public void ChangeMaterials(Material[] mat)
    {
        foreach (Renderer m in meshRenderer)
        {
            m.materials = mat;
        }
    }
    public void SetProjectileMat(Material mat)
    {
        playerMovement.projectileMat = mat;
    }

    public void SetParticles(GameObject particlePrefab)
    {
        Instantiate(particlePrefab, particlePosition.position, Quaternion.identity, particlePosition);
    }

    public void ChangeEyes(Material mat)
    {
        foreach (Renderer eye in eyes)
        {
            eye.material = mat;
        }
    }

    public void SetTrailMaterial(Material mat)
    {
        foreach (GameObject m in trails)
        {
            m.GetComponent<TrailRenderer>().material = mat;
        }
    }
}
