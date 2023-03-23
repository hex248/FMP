using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfController : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    Rigidbody rb;
    Bed bed;
    float timeSinceStart;
    GameObject currentTarget;

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
        Vector3 direction = currentTarget.transform.position - transform.position;
        DoMovement(direction, moveSpeed);
    }

    void DoMovement(Vector3 direction, float speed)
    {
        if (direction.magnitude >= 1f)
        {
            direction = direction.normalized;
        }

        Vector3 desiredVelocity = new Vector3(direction.x * speed, rb.velocity.y, direction.z * speed);
        //TODO - Make movement much smoother - Especially for the floatyness of the wolf
        rb.velocity = desiredVelocity;
    }
}
