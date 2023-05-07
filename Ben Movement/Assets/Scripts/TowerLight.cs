using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerLight : MonoBehaviour
{
    Light light;
    float minY = 1.52f;
    
    private void Start()
    {
        light = GetComponent<Light>();
    }
    void Update()
    {
        if (transform.position.y >= minY)
        {
            light.enabled = true;
        }
        else
        {
            light.enabled = false;
        }
    }
}
