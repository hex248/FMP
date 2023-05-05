using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    TeleporterManager TPM;
    public Transform spawn;
    public float offsetAmount = 0.5f;

    void Start()
    {
        TPM = FindObjectOfType<TeleporterManager>();
        TPM.AddTeleporter(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // teleport to other teleporter

            var otherTeleporter = TPM.FindNext(this);

            other.transform.position = new Vector3(otherTeleporter.spawn.position.x, other.transform.position.y, otherTeleporter.spawn.position.z) + (other.transform.forward * offsetAmount);

        }
    }
}
