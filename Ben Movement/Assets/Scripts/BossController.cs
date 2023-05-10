using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
    public float attack01Cooldown = 1.0f;
    [Header("Attack02")]
    public GameObject laserFX;
    [Header("Navmesh Settings")]
    private NavMeshAgent navMesh;

    int GetAgentTypeIDByName(string agentTypeName)
    {
        int count = NavMesh.GetSettingsCount();
        string[] agentTypeNames = new string[count + 2];
        for (var i = 0; i < count; i++)
        {
            int id = NavMesh.GetSettingsByIndex(i).agentTypeID;
            string name = NavMesh.GetSettingsNameFromID(id);
            if (name == agentTypeName)
            {
                return id;
            }
        }
        return -1;
    }

    // Start is called before the first frame update
    void Start()
    {
        animationScript = GetComponentInChildren<BossAnimationScript>();
        navMesh = GetComponent<NavMeshAgent>();
        navMesh.updatePosition = false;
        navMesh.updateRotation = false;
        navMesh.agentTypeID = GetAgentTypeIDByName("Attacking Player");
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
                col.enabled = true;
                break;
            case 1:
                MovingState();
                animationScript.SetActive(false);
                col.enabled = false;
                break;
            case 2:
                animationScript.SetActive(true);
                animationScript.Attack01();
                col.enabled = true;
                break;
            case 3:
                animationScript.SetActive(true);
                animationScript.Attack02();
                col.enabled = true;
                break;
        }
        if(currentTarget.name == "Bed")
        {
            state = 1;
        }
        else if(currentTarget.name != "Bed" && !currentTarget.GetComponent<PlayerHealth>().IsDead())
        {
            if (state < 2)
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
            if (state != previousState)
            {
                time = 0.0f;
            }
            else
            {
                if (dodgedAttacks > 2)
                {
                    if (Vector3.Scale(currentTarget.transform.position - transform.position, Vector3.one - Vector3.up).magnitude < range)
                    {
                        state = 3;
                    }
                    else
                    {
                        state = 1;
                    }
                    dodgedAttacks = 0;
                }
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
        navMesh.destination = currentTarget.transform.position;
        Vector3 direction = currentTarget.transform.position - transform.position;
        float Y = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0.0f, Mathf.LerpAngle(transform.eulerAngles.y, Y - 90.0f, Time.deltaTime * moveRotationSpeed), 0.0f);
        transform.position += transform.right * moveSpeed * Time.deltaTime * slowDownCurve.Evaluate(time);
        time += Time.deltaTime;
        if (time > 1.0f && state == previousState && state != 3)
        {
            state = 2;
        }
    }

    void MovingState()
    {
        navMesh.destination = currentTarget.transform.position;
        Vector3 direction;
        if (currentTarget.name == "Bed")
        {
            direction = (currentTarget.transform.position - transform.position).normalized;
        }
        else
        {
            direction = navMesh.desiredVelocity.normalized;
        }
        float Y = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0.0f, Mathf.LerpAngle(transform.eulerAngles.y, Y - 90.0f, Time.deltaTime * moveRotationSpeed), 0.0f);
        transform.position += transform.right * moveSpeed * Time.deltaTime * speedUpCurve.Evaluate(time);
        time += Time.deltaTime;
    }
}
