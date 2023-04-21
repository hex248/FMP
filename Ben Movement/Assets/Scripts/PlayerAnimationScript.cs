using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationScript : MonoBehaviour
{
    public float movementSpeedAnimation = 1.0f;
    private PlayerController player;
    private Animator anim;
    private Rigidbody rb;
    public bool isMoving;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponentInParent<Rigidbody>();
        player = GetComponentInParent<PlayerController>();
    }

    // Update is called once per frame

    public void StartMeleeAttackAnimation(int comboStage)
    {
        anim.SetInteger("Melee Combo Stage", comboStage);
        anim.SetTrigger("Start Melee Attack");
        anim.SetBool("isAttacking", true);
        anim.ResetTrigger("combo timeout");
        isMoving = false;
    }

    private void Update()
    {
        anim.SetBool("isMoving", isMoving);

        if(isMoving)
        {
            anim.SetBool("isAttacking", false);
        }

        anim.SetFloat("movementSpeed", rb.velocity.magnitude * 0.4f);
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

    public void ComboTimeout()
    {
        anim.SetTrigger("combo timeout");
        anim.SetBool("isAttacking", false);
    }
}
