using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float homingAmount;
    public float maxVelocity;
    public float velocity;
    public AnimationCurve velocityDecay;
    public GameObject target;
    float lifeTime = 0.0f;
    public float maxLifeTime;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        lifeTime += Time.deltaTime;
        velocity = velocityDecay.Evaluate(lifeTime / maxLifeTime);

        Quaternion targetRotation = Quaternion.LookRotation(target.transform.position - transform.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, homingAmount);
        rb.velocity = transform.forward * velocity;
    }
}
