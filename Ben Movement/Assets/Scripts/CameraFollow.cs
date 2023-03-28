using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Transform playerPoint;
    [SerializeField] Camera cam;
    [SerializeField] float smoothSpeed;
    public float hori, verti;
    Transform currentFocusPoint;
    Rigidbody rb;
    private Vector3 velocity = Vector3.zero;

    void SetObliqueness(float horizObl, float vertObl)
    {
        Matrix4x4 mat = Camera.main.projectionMatrix;
        mat[0, 2] = horizObl;
        mat[1, 2] = vertObl;
        Camera.main.projectionMatrix = mat;
    }

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
        SetObliqueness(hori, verti);
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
