using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Transform playerPoint;
    [SerializeField] Camera cam;
    [SerializeField] float smoothSpeed;
    Transform currentFocusPoint;
    Rigidbody rb;
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        ToPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentFocusPoint != null)
        {
            transform.position = Vector3.SmoothDamp(transform.position, currentFocusPoint.position, ref velocity, smoothSpeed);
        }
    }

    public void Focus(Transform focusTarget)
    {
        currentFocusPoint = focusTarget;
    }

    public void ToPlayer()
    {
        currentFocusPoint = playerPoint;
    }
}
