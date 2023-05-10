using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAnimationScript : MonoBehaviour
{
    public float movementSpeedAnimation = 1.0f;
    public float laserSpeed = 1.0f;
    public GameObject telegraph;
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
        boss.laserFX.SetActive(false);
    }
    private bool isFreezing = false;
    // Update is called once per frame
    void Update()
    {
        if (isFreezing)
        {
            LineRenderer line = ray.GetComponent<LineRenderer>();
            line.SetPosition(1, Vector3.Lerp(line.GetPosition(1), boss.currentTarget.transform.position, Time.deltaTime * laserSpeed));
            boss.laserFX.transform.position = Vector3.Lerp(line.GetPosition(1), boss.currentTarget.transform.position, Time.deltaTime * laserSpeed);
            Physics.Raycast(line.GetPosition(0), line.GetPosition(1) - line.GetPosition(0), out RaycastHit hit);
            Collider[] cols = Physics.OverlapSphere(hit.point, 2.0f);
            foreach(Collider col in cols)
            {
                if (col.CompareTag("Player"))
                {
                    col.GetComponent<PlayerHealth>().Damage(1);
                }
            }
        }
    }
    public void StartTelegraph()
    {
        telegraph.SetActive(true);
    }

    public void EndTelegraph()
    {
        telegraph.SetActive(false);
    }

    public void StartBeam()
    {
        if (boss.currentTarget != null)
        {
            Vector3 direction = (boss.currentTarget.transform.position - transform.position).normalized;
            float Y = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            boss.transform.eulerAngles = new Vector3(0.0f, Y - 90.0f, 0.0f);
            ray.SetActive(true);
            LineRenderer line = ray.GetComponent<LineRenderer>();
            line.SetPosition(0, ray.transform.position);
            line.SetPosition(1, boss.currentTarget.transform.position);
            isFreezing = true;
            boss.laserFX.SetActive(true);
        }
    }

    public void EndBeam()
    {
        ray.SetActive(false);
        isFreezing = false;
        boss.laserFX.SetActive(false);
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
        if(boss.currentTarget != null)
        {
            if (!arm.activeInHierarchy)
            {
                EndBeam();
                anim.SetTrigger("attack01");
                if ((previousPos - boss.currentTarget.transform.position).magnitude > 1.0f)
                {
                    arm.transform.position = boss.currentTarget.transform.position - (boss.currentTarget.transform.right);
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
    }

    public void SetActive(bool active)
    {
        anim.SetBool("isActive", active);
    }

    public void StartMoving()
    {
        EndBeam();
        anim.SetBool("isMoving", true);
    }

    public void EndMoving()
    {
        anim.SetBool("isMoving", false);
    }
}
