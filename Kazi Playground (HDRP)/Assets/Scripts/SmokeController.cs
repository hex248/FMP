using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[ExecuteAlways]
public class SmokeController : MonoBehaviour
{
    WorldManager wm;
    VisualEffect effect;
    void Start()
    {
        wm = FindObjectOfType<WorldManager>();
        effect = GetComponent<VisualEffect>();
    }

    void Update()
    {
        effect.SetVector3("WindDirection", wm.wind.rotation * Vector3.up);
    }
}
