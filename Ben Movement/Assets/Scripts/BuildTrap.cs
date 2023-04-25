using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildTrap : MonoBehaviour
{
    [SerializeField] GameObject interactPopup;
    [SerializeField] GameObject hologramParent;

    [SerializeField] GameObject[] holograms;
    [SerializeField] GameObject[] prefabs;
    GameObject currentHologram;
    int hologramIDX = 0;

    float timeSinceCycleLeft = 0.0f;
    float timeSinceCycleRight = 0.0f;
    public float cycleCooldown = 0.25f;

    private void Start()
    {
        interactPopup.SetActive(false);
        hologramParent.SetActive(false);
    }

    private void Update()
    {
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
            // if is interacting
            if (other.GetComponent<PlayerController>().interacting)
            {
                if (!hologramParent.activeInHierarchy)
                {
                    ShowHolograms();
                }
                else // if interacting while holograms shown:
                {
                    Build(prefabs[hologramIDX]);
                }
            }
            // otherwise, show interact icon if the holograms aren't showing
            else if (!hologramParent.activeInHierarchy)
            {
                ShowInteractPopup();
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
            HideInteractPopup();
            HideHolograms();
        }
    }

    void Build(GameObject prefab)
    {

    }

    void ShowInteractPopup()
    {
        interactPopup.SetActive(true);
        hologramParent.SetActive(false);
    }

    void ShowHolograms()
    {
        interactPopup.SetActive(false);
        hologramParent.SetActive(true);
    }
    void HideInteractPopup()
    {
        interactPopup.SetActive(false);
    }

    void HideHolograms()
    {
        hologramParent.SetActive(false);
    }

    void CycleLeft()
    {
        hologramIDX--;
        if (hologramIDX <= 0) hologramIDX = holograms.Length - 1;
    }

    void CycleRight()
    {
        hologramIDX++;
        if (hologramIDX >= holograms.Length) hologramIDX = 0;
    }
}
