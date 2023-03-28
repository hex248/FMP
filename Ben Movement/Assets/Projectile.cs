using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Movement Settings")]
    public float projectileSpeed;
    public float homingAmount;
    public GameObject currentTarget;
    [Header("Attack Settings")]
    public RangedAttack attackInfo;
    

    Collider col;
    Rigidbody rb;

    private void Start()
    {
        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
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
            enemy.TakeDamage(attackInfo.damage);
            Debug.Log("deal " + attackInfo.damage + " damage");
        }

        Rigidbody hitRb = collision.gameObject.GetComponent<Rigidbody>();
        if (hitRb != null)
        {
            hitRb.AddForce(transform.forward * attackInfo.force);
        }

        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        //RotateTowardsTarget();
        MoveForward(projectileSpeed);
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
