using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationScript : MonoBehaviour
{
    public float movementSpeedAnimation = 1.0f;
    private Animator anim;
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponentInParent<Rigidbody>();
    }

    private Vector3 direction;

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
        if (Vector3.Scale(rb.velocity, Vector3.one - Vector3.up).magnitude > 0.0f)
        {
            anim.SetBool("isMoving", true);
            anim.SetFloat("movementSpeed", Vector3.Scale(rb.velocity, Vector3.one - Vector3.up).magnitude * movementSpeedAnimation);
            direction = Vector3.Scale(rb.velocity, Vector3.one - Vector3.up).normalized;
        }
        else
        {
            anim.SetBool("isMoving", false);
        }
    }
}
