using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildTrap : MonoBehaviour
{
    public GameObject interactPopup;
    public GameObject buildPopup;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ShowInteractPopup();

            // press interact bind

            // if is interacting

            if (other.GetComponent<PlayerController>().interacting)
            {
                ShowBuildPopup();
            }
        }
    }

    void ShowInteractPopup()
    {

    }

    void ShowBuildPopup()
    {

    }
}
