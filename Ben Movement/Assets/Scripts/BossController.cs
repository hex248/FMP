using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] AnimationCurve speedUpCurve;
    [SerializeField] AnimationCurve slowDownCurve;
    [SerializeField] float moveSpeed = 1.0f;
    public float moveRotationSpeed = 1.0f;
    [SerializeField] float range = 1.0f;
    [HideInInspector]
    public GameObject currentTarget;
    public Collider col;
    private BossAnimationScript animationScript;
    [Header("Attack01")]
    [SerializeField] GameObject attack01;
    [SerializeField] GameObject shadow;
    public float attack01Cooldown = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        animationScript = GetComponentInChildren<BossAnimationScript>();
    }

    private int state = 0;
    [HideInInspector]
    //[Kazi] Checking for if attack is dodged isn't working atm
    public int dodgedAttacks = 0;
    private float time = 0.0f;
    private int previousState;

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (state)
        {
            case 0:
                SlowDown();
                animationScript.SetActive(true);
                shadow.transform.localScale = Vector3.one * slowDownCurve.Evaluate(time) * 7.5f;
                break;
            case 1:
                MovingState();
                animationScript.SetActive(false);
                shadow.transform.localScale = Vector3.one * speedUpCurve.Evaluate((time - 0.75f) / 10.0f) * 7.5f;
                break;
            case 2:
                animationScript.SetActive(true);
                animationScript.Attack01();
                Vector3 direction = (currentTarget.transform.position - transform.position).normalized;
                float Y = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
                transform.eulerAngles = new Vector3(0.0f, Mathf.LerpAngle(transform.eulerAngles.y, Y - 90.0f, Time.deltaTime * moveRotationSpeed), 0.0f);
                shadow.transform.localScale = Vector3.zero;
                col.enabled = true;
                break;
            case 3:
                animationScript.SetActive(true);
                animationScript.Attack02();
                shadow.transform.localScale = Vector3.zero;
                col.enabled = true;
                break;
        }
        if(state < 2)
        {
            if (currentTarget != null && Vector3.Scale(currentTarget.transform.position - transform.position, Vector3.one - Vector3.up).magnitude > range)
            {
                state = 1;
            }
            else
            {
                state = 0;
            }
        }
        if(state != previousState)
        {
            time = 0.0f;
        }
        else
        {
            if (dodgedAttacks > 2)
            {
                state = 3;
                dodgedAttacks = 0;
            }
        }
        previousState = state;
    }

    public void SetState(int newState)
    {
        state = newState;
    }

    void SlowDown()
    {
        transform.position += transform.right * moveSpeed * Time.deltaTime * slowDownCurve.Evaluate(time);
        time += Time.deltaTime;
        if (time > 1.0f && state == previousState && state != 3)
        {
            state = 2;
        }
        col.enabled = true;
    }

    void MovingState()
    {
        Vector3 direction = (currentTarget.transform.position - transform.position).normalized;
        float Y = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0.0f, Mathf.LerpAngle(transform.eulerAngles.y, Y - 90.0f, Time.deltaTime * moveRotationSpeed), 0.0f);
        transform.position += transform.right * moveSpeed * Time.deltaTime * speedUpCurve.Evaluate(time);
        time += Time.deltaTime;
        col.enabled = false;
    }
}
