using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BuildTrap : MonoBehaviour
{
    AudioManager AM;
    TeleporterManager TPM;
    Bed bed;

    [SerializeField] GameObject interactControls;
    [SerializeField] GameObject hologramControls;
    [SerializeField] TextMeshProUGUI buildCostText;
    [SerializeField] GameObject hologramParent;

    [SerializeField] List<GameObject> holograms;
    [SerializeField] List<GameObject> prefabs;
    [SerializeField] List<int> costs;
    GameObject currentHologram;
    int hologramIDX = 0;

    float timeSinceInteract = 0.0f;
    float timeSinceCycleLeft = 0.0f;
    float timeSinceCycleRight = 0.0f;
    public float interactCooldown = 1.0f;
    public float cycleCooldown = 0.25f;

    public PlayerTrigger interactingPlayerTrigger;
    public PlayerController interactingPlayer;

    bool hasBeenBuilt = false;

    private void Start()
    {
        AM = FindObjectOfType<AudioManager>();
        TPM = FindObjectOfType<TeleporterManager>();
        bed = FindObjectOfType<Bed>();
        HideInteractControls();
        HideHologramControls();
        HideHolograms();
    }

    private void Update()
    {
        if (interactingPlayer != null)
        {
            // set control icons
            //Debug.Log(interactingPlayerTrigger.player.playerInput.currentActionMap.controlSchemes);
            //Debug.Log(interactingPlayerTrigger.player.playerInput.currentActionMap.devices);
            string deviceName = "";
            string deviceManufacturer = "";
            string controlPath = "";
            var devices = interactingPlayerTrigger.player.playerInput.currentActionMap.devices;
            foreach (var device in devices)
            {
                deviceName = device.name;
                deviceManufacturer = device.description.manufacturer;
            }
            //Debug.Log(interactingPlayerTrigger.player.playerInput.currentActionMap.FindAction("Interact").type);
            var bindings = interactingPlayerTrigger.player.playerInput.currentActionMap.FindAction("Interact").bindings.ToArray();
            foreach(var binding in bindings)
            {
                if (binding.path.Contains(deviceName))
                {
                    controlPath = binding.path.Split('/')[1];
                }
            }
            Debug.Log($"{deviceName}: {controlPath}");

            InputIcons.Icons icons = InputIcons.instance.keyboard; // keyboard is default

            switch (deviceName)
            {
                case "Keyboard":
                    icons = InputIcons.instance.keyboard;
                    break;
                case "Gamepad":
                    switch(deviceManufacturer)
                    {
                        case "Sony Interactive Entertainment":
                            icons = InputIcons.instance.ps4;
                            break;
                        case "Microsoft":
                            icons = InputIcons.instance.xbox;
                            break;
                    }
                break;
            }

            Sprite interactSprite = icons.GetSprite(controlPath);
            Debug.Log(interactSprite);
        }
        timeSinceInteract += Time.deltaTime;
        timeSinceCycleLeft += Time.deltaTime;
        timeSinceCycleRight += Time.deltaTime;
        if (hologramParent.activeInHierarchy)
        {
            if (currentHologram != null && currentHologram != holograms[hologramIDX]) currentHologram.SetActive(false);

            currentHologram = holograms[hologramIDX];
            holograms[hologramIDX].SetActive(true);
            int towerCost = costs[hologramIDX];
            buildCostText.text = $"{towerCost}";

            if (towerCost > Essence.instance.balance) buildCostText.color = Color.red;
            else buildCostText.color = Color.green;
            
            var lookDirection = (bed.transform.position - holograms[hologramIDX].transform.position).normalized;
            lookDirection.y = 0.0f;
            holograms[hologramIDX].transform.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player Trigger"))
        {
            // if there isnt already a player interacting with this
            if (interactingPlayer == null)
            {
                PlayerTrigger pt = other.GetComponent<PlayerTrigger>();
                interactingPlayerTrigger = pt;
                interactingPlayer = pt.controller;
                pt.controller.inInteractRange = true;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player Trigger"))
        {
            PlayerTrigger pt = other.GetComponent<PlayerTrigger>();
            if (pt.controller != interactingPlayer) return;
            //ShowInteractControls();
            // if is interacting
            if (pt.controller.interacting)
            {
                if (timeSinceInteract >= interactCooldown)
                {
                    timeSinceInteract = 0.0f;
                    if (!hologramParent.activeInHierarchy)
                    {
                        ShowHolograms();
                        ShowHologramControls();
                        HideInteractControls();
                    }
                }
            }
            else
            {
                HideHologramControls();
                HideHolograms();
                ShowInteractControls();
            }

            if (pt.controller.building && Essence.instance.balance >= costs[hologramIDX])
            {
                if(!hasBeenBuilt)
                {
                    Essence.instance.balance -= costs[hologramIDX];
                    Build(prefabs[hologramIDX]);
                    hasBeenBuilt = true;
                }
            }

            if (pt.controller.cycleLeftPressed)
            {
                if (timeSinceCycleLeft >= cycleCooldown)
                {
                    timeSinceCycleLeft = 0.0f;
                    CycleLeft();
                }
            }
            else if (pt.controller.cycleRightPressed)
            {
                if (timeSinceCycleRight >= cycleCooldown)
                {
                    timeSinceCycleRight = 0.0f;
                    CycleRight();
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player Trigger"))
        {
            if (other.GetComponent<PlayerTrigger>().controller != interactingPlayer) return;
            HideInteractControls();
            HideHologramControls();


            interactingPlayer.inInteractRange = false;


            HideHolograms();
            interactingPlayerTrigger = null;
            interactingPlayer = null;
            
        }
    }

    void Build(GameObject prefab)
    {
        // play build sound effect
        if (AM.buildSoundOn)
        {
            AM.PlayInChannel("tower_build", ChannelType.SFX, 1);
        }
        // spawn tower
        var spawned = Instantiate(prefab);
        spawned.transform.position = hologramParent.transform.position;
        var lookDirection = (bed.transform.position - spawned.transform.position).normalized;
        lookDirection.y = 0.0f;
        spawned.transform.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
        // destroy self
        Destroy(gameObject);
    }

    void ShowInteractControls()
    {
        interactControls.SetActive(true);
        interactControls.layer = LayerMask.NameToLayer($"OnlyPlayer{interactingPlayer.playerNumber}");
    }

    void ShowHologramControls()
    {
        hologramControls.SetActive(true);
        foreach (Transform child in hologramControls.transform)
        {
            child.gameObject.layer = LayerMask.NameToLayer($"OnlyPlayer{interactingPlayer.playerNumber}");
        }
    }

    void ShowHolograms()
    {
        hologramParent.SetActive(true);
    }
    void HideInteractControls()
    {
        interactControls.SetActive(false);
    }

    void HideHologramControls()
    {
        hologramControls.SetActive(false);
    }

    void HideHolograms()
    {
        hologramParent.SetActive(false);
    }

    void CycleLeft()
    {
        hologramIDX--;
        if (hologramIDX < 0) hologramIDX = holograms.Count - 1;
    }

    void CycleRight()
    {
        hologramIDX++;
        if (hologramIDX >= holograms.Count) hologramIDX = 0;
    }
}
