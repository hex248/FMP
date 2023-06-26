using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Movement Settings")]
    public float projectileSpeed;
    public float homingAmount;
    public float lifeTime = 5.0f;
    public GameObject currentTarget;
    [Header("Attack Settings")]
    public float damage;
    public float force;
    public float scaleMultiplier;

    [Header("Owner Settings")]
    public Player owner;

    TrailRenderer trail;

    Collider col;
    Rigidbody rb;

    private void Start()
    {
        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        trail = GetComponent<TrailRenderer>();
        transform.localScale = transform.localScale * scaleMultiplier;
        trail.startWidth = trail.startWidth * scaleMultiplier;

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject == null)
        {
            return;
        }
        EnemyHealth enemy = collision.gameObject.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage, owner.gameObject);
        }

        Rigidbody hitRb = collision.gameObject.GetComponent<Rigidbody>();
        if (hitRb != null)
        {
            hitRb.AddForce(transform.forward * force);
        }

        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        //RotateTowardsTarget();
        MoveForward(projectileSpeed);
        if(lifeTime > 0.0f)
        {
            lifeTime -= Time.deltaTime;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void MoveForward(float speed)
    {
        Vector3 desiredVelocity = transform.forward * projectileSpeed;
        rb.velocity = desiredVelocity;
    }

    void RotateTowardsTarget()
    {
        Quaternion targetRotation = Quaternion.LookRotation(currentTarget.transform.position - transform.position, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, homingAmount * Time.deltaTime);
    }
}
