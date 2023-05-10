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
        transform.localPosition = new Vector3(0f, -1f + 0.2f * Mathf.Sin(Time.time), 0f);
        transform.eulerAngles += new Vector3(0f, Time.deltaTime * 30f, 0f);
    }
}
