using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAnimationScript : MonoBehaviour
{
    public float movementSpeedAnimation = 1.0f;
    public GameObject arm;
    public GameObject ray;
    private BossController boss;
    private Animator anim;
    private Rigidbody rb;
    float coolDown;
    private Vector3 previousPos;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponentInParent<Rigidbody>();
        boss = GetComponentInParent<BossController>();
        coolDown = boss.attack01Cooldown;
        ray.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartBeam()
    {
        Vector3 direction = (boss.currentTarget.transform.position - transform.position).normalized;
        float Y = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        boss.transform.eulerAngles = new Vector3(0.0f, Y - 90.0f, 0.0f);
        ray.SetActive(true);
        ray.GetComponent<LineRenderer>().SetPosition(1, boss.currentTarget.transform.position);
    }

    public void EndBeam()
    {
        ray.SetActive(false);
    }

    private void LateUpdate()
    {
        previousPos = boss.currentTarget.transform.position;
    }

    public void Attack02()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            anim.SetTrigger("attack02");
        }
        else
        {
            anim.ResetTrigger("attack02");
            boss.SetState(1);
        }
    }

    int previousHealth;

    PlayerHealth target;

    public void Attack01()
    {
        if (!arm.activeInHierarchy)
        {
            anim.SetTrigger("attack01");
            if((previousPos - boss.currentTarget.transform.position).magnitude > 1.0f)
            {
                arm.transform.position = boss.currentTarget.transform.position + (boss.currentTarget.transform.forward * 1.5f);
            }
            else
            {
                arm.transform.position = boss.currentTarget.transform.position;
            }
        }
        if (arm.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            arm.SetActive(true);
        }
        else
        {
            boss.dodgedAttacks++;
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
