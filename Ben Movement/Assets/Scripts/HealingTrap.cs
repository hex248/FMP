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
        if (other.CompareTag("Player Trigger"))
        {
            // play healing sound in SFX Channel 1
            AM.PlayInChannel("healing-trap_heal", ChannelType.SFX, 1);
            other.GetComponent<PlayerTrigger>().health.Heal(1);
        }
    }
}
