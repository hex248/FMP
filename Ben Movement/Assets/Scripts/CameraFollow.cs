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
    Vector3 smoothFocusPoint;

    void SetObliqueness(float horizObl, float vertObl)
    {
        Matrix4x4 mat = cam.projectionMatrix;
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
            smoothFocusPoint = Vector3.SmoothDamp(smoothFocusPoint, currentFocusPoint.position, ref velocity, smoothSpeed);
            transform.position = smoothFocusPoint + shakeOffset;
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

    public void CameraShake(float strength, float time)
    {
        StartCoroutine(DoCameraShake(strength, time));
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
