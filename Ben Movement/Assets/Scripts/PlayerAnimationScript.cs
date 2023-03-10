using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationScript : MonoBehaviour
{
    public float movementSpeedAnimation = 1.0f;
    private PlayerController player;
    private Animator anim;
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponentInParent<Rigidbody>();
        player = GetComponentInParent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Scale(rb.velocity, Vector3.one - Vector3.up).magnitude > 0.5f)
        {
            anim.SetBool("isMoving", true);
            anim.SetFloat("movementSpeed", Vector3.Scale(rb.velocity, Vector3.one - Vector3.up).magnitude * movementSpeedAnimation);
        }
        else
        {
            anim.SetBool("isMoving", false);
        }
        if (player.movementAttackLocked)
        {
            anim.SetTrigger("StartAttack");
        }
        else
        {
            anim.ResetTrigger("StartAttack");
        }
    }
}
