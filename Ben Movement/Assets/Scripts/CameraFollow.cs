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
    Vector3 shakeOffset;

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

    public void CameraShake()
    {
        StartCoroutine(DoCameraShake(10f, 1f));
    }

    IEnumerator DoCameraShake(float strength, float time)
    {
        float timeSinceShakeStart = 0f;
        while(timeSinceShakeStart < time)
        {
            timeSinceShakeStart += Time.deltaTime;

            shakeOffset = new Vector3(Random.Range(-strength, strength), 0f, Random.Range(-strength, strength));
            yield return null;
        }
        shakeOffset = new Vector3(0f, 0f, 0f);
    }
}
