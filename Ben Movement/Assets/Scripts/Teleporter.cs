using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    AudioManager AM;
    TeleporterManager TPM;
    public Transform spawn;
    public float offsetAmount = 0.5f;

    [Header("Visuals")]
    public GameObject[] lights;
    public GameObject visuals;
    Vector3 towerPos;

    void Start()
    {
        AM = FindObjectOfType<AudioManager>();
        TPM = FindObjectOfType<TeleporterManager>();
        TPM.AddTeleporter(this);
        towerPos = visuals.transform.position;
        StartCoroutine(SpawnTurret());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player Trigger"))
        {
            AM.PlayInChannel("teleporter_teleport", ChannelType.SFX, 1);
            var pt = other.GetComponent<PlayerTrigger>();

            // teleport to other teleporter

            var otherTeleporter = TPM.FindNext(this);

            pt.controller.transform.position = new Vector3(otherTeleporter.spawn.position.x, pt.controller.transform.position.y, otherTeleporter.spawn.position.z) + (pt.controller.transform.forward * offsetAmount);

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
