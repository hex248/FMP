using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform target;
    public float moveSpeed;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // move towards center
        rb.velocity = moveSpeed * Time.deltaTime * 100.0f * Vector3.Scale((target.position - transform.position).normalized, Vector3.one - Vector3.up) + Vector3.up * rb.velocity.y;
    }
}
