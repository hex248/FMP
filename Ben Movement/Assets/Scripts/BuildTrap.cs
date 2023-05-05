using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildTrap : MonoBehaviour
{
    AudioManager AM;
    TeleporterManager TPM;

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

    private void Start()
    {
        AM = FindObjectOfType<AudioManager>();
        TPM = FindObjectOfType<TeleporterManager>();
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
            holograms[hologramIDX].SetActive(true);
            currentHologram = holograms[hologramIDX];
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ShowInteractControls();
            // if is interacting
            if (other.GetComponent<PlayerController>().interacting)
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

            if (other.GetComponent<PlayerController>().cycleLeftPressed)
            {
                if (timeSinceCycleLeft >= cycleCooldown)
                {
                    timeSinceCycleLeft = 0.0f;
                    CycleLeft();
                }
            }
            else if (other.GetComponent<PlayerController>().cycleRightPressed)
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
        if (other.CompareTag("Player"))
        {
            HideInteractControls();
            HideHologramControls();
            HideHolograms();
        }
    }

    void Build(GameObject prefab)
    {
        // play build sound effect
        //AM.PlayInChannel("tower_build", ChannelType.SFX, 1);
        // spawn tower
        var spawned = Instantiate(prefab);
        spawned.transform.position = hologramParent.transform.position;
        // destroy self
        Destroy(gameObject);
    }

    void ShowInteractControls()
    {
        interactControls.SetActive(true);
    }

    void ShowHologramControls()
    {
        hologramControls.SetActive(true);
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
