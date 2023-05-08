using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingTrap : MonoBehaviour
{
    AudioManager AM;

    [Header("Visuals")]
    public GameObject[] lights;
    public GameObject visuals;
    Vector3 towerPos;
    void Start()
    {
        AM = FindObjectOfType<AudioManager>();
        towerPos = visuals.transform.position;
        StartCoroutine(SpawnTurret());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player Trigger"))
        {
            // play healing sound in SFX Channel 1
            if (AM.healingSoundOn)
            {
                AM.PlayInChannel("healing-trap_heal", ChannelType.SFX, 1);
            }
            other.GetComponent<PlayerTrigger>().health.Heal(1);
        }
    }

    IEnumerator SpawnTurret()
    {
        visuals.transform.position = new Vector3(towerPos.x, -10.0f, towerPos.z);
        foreach (GameObject light in lights)
        {
            light.SetActive(false);
        }

        float moveSpeed = 5.0f;
        float distanceToMove = (towerPos - visuals.transform.position).sqrMagnitude;

        for (; ; )
        {
            // move towards turretPos

            visuals.transform.position = Vector3.Lerp(visuals.transform.position, towerPos, moveSpeed * Time.deltaTime);

            if (visuals.transform.position == towerPos)
            {
                break;
            }

            yield return new WaitForEndOfFrame();
        }

        foreach (GameObject light in lights)
        {
            light.SetActive(true);
        }
    }
}
