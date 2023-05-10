using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DreamEssencePickup : MonoBehaviour
{
    public int essenceAmount = 1;
    public float bobAmount = 10f;
    public float bobSpeed = 1.0f;

    EssenceManager EM;
    AudioManager AM;

    private void Start()
    {
        EM = FindObjectOfType<EssenceManager>();
        AM = FindObjectOfType<AudioManager>();
    }

    void Update()
    {
        transform.position = new Vector3(transform.position.x, 2.0f + bobAmount/100 * Mathf.Sin(Time.time * bobSpeed), transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            EM.Essence += essenceAmount;
            if (AM.pickupSoundOn)
            {
                AM.PlayInChannel("pickup_pop", ChannelType.SFX, 2);
            }
            Destroy(this.gameObject);
        }
    }
}
