using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingTrap : MonoBehaviour
{
    AudioManager AM;
    void Start()
    {
        AM = FindObjectOfType<AudioManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // play healing sound in SFX Channel 1
            //healing-trap_heal
            AM.PlayInChannel("UI_button-press", ChannelType.SFX, 1); //! IMPORTANT - CHANGE THIS  SOUND ONCE HEALING SOUND HAS BEEN IMPORTED
        }
    }
}
