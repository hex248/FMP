using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SnowDust : MonoBehaviour
{
    VisualEffect VFX;

    private void Start()
    {
        VFX = GetComponent<VisualEffect>();
    }

    private void Update()
    {
        VFX.SetVector3("SpawnPosition", transform.position);
        VFX.SetVector3("EmissionDirection", transform.forward * -1);
    }

    public void OnDash()
    {
        VFX.SendEvent("OnDash");
    }

    public void OnDashEnd()
    {
        VFX.SendEvent("OnDashEnd");
    }
}
