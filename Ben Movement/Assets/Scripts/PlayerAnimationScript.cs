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

        anim.SetFloat("movementSpeed", rb.velocity.magnitude * movementSpeedAnimation);
    }

    public void SetRangedAttackStrength(float strength)
    {
        anim.SetFloat("RangedAttackStrength", strength);
    }

    public void StartRangedAttackAnimation()
    {
        anim.SetBool("isRangedAttacking", true);
    }

    public void EndRangedAttackAnimation()
    {
        anim.SetBool("isRangedAttacking", false);
    }

    public void StartDashAnimation()
    {
        anim.SetBool("dodge", true);
        anim.SetBool("isAttacking", false);
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

    public void Damage()
    {
        anim.SetTrigger("Damaged");
        anim.SetBool("isAttacking", false);
    }
}
