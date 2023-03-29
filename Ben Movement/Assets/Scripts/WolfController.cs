using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfController : MonoBehaviour
{
    [SerializeField] GameObject wolfVisuals;
    [SerializeField] float moveSpeed;
    [SerializeField] float rotationSpeed;
    Rigidbody rb;
    Bed bed;
    float timeSinceStart;
    GameObject currentTarget;
    [SerializeField] bool targetPlayer;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        bed = FindObjectOfType<Bed>();
        currentTarget = bed.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        RotateTowardsTarget();
        MoveForward(moveSpeed);
        Vector3 direction = currentTarget.transform.position - transform.position;

        if (targetPlayer)
        {
            targetPlayer = false;
            currentTarget = FindObjectOfType<PlayerController>().gameObject;
        }
    }

    void MoveForward(float speed)
    {
        Vector3 desiredVelocity = wolfVisuals.transform.forward * moveSpeed;
        rb.velocity = desiredVelocity;
    }

    void RotateTowardsTarget()
    {
        Quaternion targetRotation = Quaternion.LookRotation(currentTarget.transform.position - transform.position, Vector3.up);
        transform.rotation = Quaternion.Lerp(wolfVisuals.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        //smoke.SetVector3("EmissionDirection", head.forward * -1);
    }

    void Attack()
    {

    }
}
