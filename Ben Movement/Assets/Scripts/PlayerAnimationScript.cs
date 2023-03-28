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
        Vector3 movement = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if (movement.magnitude > 0.5f)
        {
            anim.SetBool("isMoving", true);
            anim.SetFloat("movementSpeed", movement.magnitude * movementSpeedAnimation);
        }
        else
        {
            anim.SetBool("isMoving", false);
        }
    }

    public void StartMeleeAttackAnimation(int comboStage)
    {
        anim.SetInteger("Melee Combo Stage", comboStage);
        anim.SetTrigger("Start Melee Attack");
    }

    public void StartRangedAttackAnimation()
    {
        anim.SetTrigger("Start Ranged Attack");
    }

    public void StartDashAnimation()
    {
        anim.SetBool("dodge", true);
    }

    public void EndDashAnimation()
    {
        anim.SetBool("dodge", false);
    }
}
