using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildTrap : MonoBehaviour
{
    AudioManager AM;
    TeleporterManager TPM;
    Bed bed;

    [SerializeField] GameObject interactControls;
    [SerializeField] GameObject hologramControls;
    [SerializeField] GameObject hologramParent;

    [SerializeField] List<GameObject> holograms;
    [SerializeField] List<GameObject> prefabs;
    GameObject currentHologram;
    int hologramIDX = 0;

    float timeSinceInteract = 0.0f;
    float timeSinceCycleLeft = 0.0f;
    float timeSinceCycleRight = 0.0f;
    public float interactCooldown = 1.0f;
    public float cycleCooldown = 0.25f;

    public PlayerController interactingPlayer;

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
        timeSinceInteract += Time.deltaTime;
        timeSinceCycleLeft += Time.deltaTime;
        timeSinceCycleRight += Time.deltaTime;
        if (hologramParent.activeInHierarchy)
        {
            if (currentHologram != null && currentHologram != holograms[hologramIDX]) currentHologram.SetActive(false);

            currentHologram = holograms[hologramIDX];
            holograms[hologramIDX].SetActive(true);

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
                interactingPlayer = pt.controller;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player Trigger"))
        {
            PlayerTrigger pt = other.GetComponent<PlayerTrigger>();
            if (pt.controller != interactingPlayer) return;
            ShowInteractControls();
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
                    }
                    else // if interacting while holograms shown:
                    {
                        Build(prefabs[hologramIDX]);
                    }
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
            HideHolograms();
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
