using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, 1.5f + 0.2f * Mathf.Sin(Time.time), transform.position.z);
        transform.eulerAngles += new Vector3(0f, Time.deltaTime * 30f, 0f);
    }
}
