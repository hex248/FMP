using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildTrap : MonoBehaviour
{
    public GameObject interactPopup;
    public GameObject buildPopup;

    private void Start()
    {
        interactPopup.SetActive(false);
        buildPopup.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            // press interact bind

            // if is interacting

            Debug.Log(other.GetComponent<PlayerController>().interacting);
            if (other.GetComponent<PlayerController>().interacting)
            {
                ShowBuildPopup();
            }
            else if (!buildPopup.activeInHierarchy)
            {
                ShowInteractPopup();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HideInteractPopup();
            HideBuildPopup();
        }
    }

    void ShowInteractPopup()
    {
        interactPopup.SetActive(true);
        buildPopup.SetActive(false);
    }

    void ShowBuildPopup()
    {
        interactPopup.SetActive(false);
        buildPopup.SetActive(true);
    }
    void HideInteractPopup()
    {
        interactPopup.SetActive(false);
    }

    void HideBuildPopup()
    {
        buildPopup.SetActive(false);
    }
}
