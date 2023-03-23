using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[ExecuteAlways]
public class SmokeController : MonoBehaviour
{
    WorldManager wm;
    void Start()
    {
        wm = FindObjectOfType<WorldManager>();
    }

    void Update()
    {
        GetComponent<VisualEffect>().SetVector3("WindDirection", wm.wind.rotation * Vector3.up);
    }
}
