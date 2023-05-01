using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAnimationScript : MonoBehaviour
{
    public float movementSpeedAnimation = 1.0f;
    public GameObject arm;
    private BossController boss;
    private Animator anim;
    private Rigidbody rb;
    float coolDown;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponentInParent<Rigidbody>();
        boss = GetComponentInParent<BossController>();
        coolDown = boss.attack01Cooldown;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Attack01()
    {
        if (!arm.activeInHierarchy)
        {
            arm.transform.position = boss.currentTarget.transform.position;
        }
        if (arm.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            arm.SetActive(true);
        }
        else
        {
            arm.SetActive(false);
            boss.SetState(1);
        }
    }

    public void SetActive(bool active)
    {
        anim.SetBool("isActive", active);
    }

    public void StartMoving()
    {
        anim.SetBool("isMoving", true);
    }

    public void EndMoving()
    {
        
        anim.SetBool("isMoving", false);
    }
}
