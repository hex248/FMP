using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfAnimationScript : MonoBehaviour
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

    // Update is called once per frame
    void Update()
    {
        Vector3 movement = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        anim.SetFloat("movementSpeed", movement.magnitude * movementSpeedAnimation);
    }

    public void TakeDamageAnimation()
    {
        anim.SetTrigger("damage");
    }

    public void StartAttackAnimation()
    {
        anim.SetTrigger("attack");
    }

    public void PrepareAttack()
    {
        anim.SetTrigger("prepare attack");
    }

    public void AttackHit()
    {
        anim.SetTrigger("attack complete");
        anim.SetBool("missed attack", false);
    }

    public void AttackMissed()
    {
        anim.SetTrigger("attack complete");
        anim.SetBool("missed attack", true);
    }
}
