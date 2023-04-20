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
    }

    private void Update()
    {
        anim.SetBool("isMoving", isMoving);
        anim.SetFloat("movementSpeed", rb.velocity.magnitude * movementSpeedAnimation);
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
