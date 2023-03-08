using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Transform focusPoint;
    [SerializeField] Camera cam;
    [SerializeField] float smoothSpeed;
    Rigidbody rb;
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.SmoothDamp(transform.position, focusPoint.position, ref velocity, smoothSpeed);
    }
}
