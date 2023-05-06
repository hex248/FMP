using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        transform.position = new Vector3(transform.position.x, 1.0f + 0.2f * Mathf.Sin(Time.time), transform.position.z);
        transform.eulerAngles += new Vector3(0f, Time.deltaTime * 30f, 0f);
    }
}
