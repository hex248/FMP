using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildTrap : MonoBehaviour
{
    public GameObject interactPopup;
    public GameObject hologramParent;

    public GameObject[] holograms;
    public GameObject currentHologram;
    public int hologramIDX = 0;

    private void Start()
    {
        interactPopup.SetActive(false);
        hologramParent.SetActive(false);
    }

    private void Update()
    {
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
                ShowHolograms();
            }
            // otherwise, show interact icon if the holograms aren't showing
            else if (!hologramParent.activeInHierarchy)
            {
                ShowInteractPopup();
            }

            if (other.GetComponent<PlayerController>().cycleLeftPressed)
            {
                CycleLeft();
            }
            else if (other.GetComponent<PlayerController>().cycleRightPressed)
            {
                CycleRight();
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
