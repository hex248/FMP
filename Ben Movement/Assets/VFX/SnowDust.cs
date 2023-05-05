using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SnowDust : MonoBehaviour
{
    VisualEffect VFX;
    public float spawnAmount = 15.0f;
    public Transform spawnTransform;

    private void Start()
    {
        VFX = GetComponent<VisualEffect>();
        VFX.SetFloat("Spawn Amount", 0.0f);
    }

    private void Update()
    {
        VFX.SetVector3("SpawnPosition", spawnTransform.position);
        VFX.SetVector3("EmissionDirection", spawnTransform.forward * -1);
    }

    public void OnDash()
    {
        VFX.SetFloat("Spawn Amount", spawnAmount);
    }

    public void OnDashEnd()
    {
        VFX.SetFloat("Spawn Amount", 0.0f);
    }
}
