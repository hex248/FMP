using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] AnimationCurve speedUpCurve;
    [SerializeField] AnimationCurve slowDownCurve;
    [SerializeField] float moveSpeed = 1.0f;
    [SerializeField] float moveRotationSpeed = 1.0f;
    [SerializeField] float range = 1.0f;
    [HideInInspector]
    public GameObject currentTarget;
    private BossAnimationScript animationScript;
    // Start is called before the first frame update
    void Start()
    {
        animationScript = GetComponentInChildren<BossAnimationScript>();
    }

    private int state = 0;
    private float time = 0.0f;
    private int previousState;

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (state)
        {
            case 0:
                SlowDown();
                animationScript.EndMoving();
                break;
            case 1:
                MovingState();
                animationScript.StartMoving();
                break;
        }
        if(Vector3.Scale(currentTarget.transform.position - transform.position, Vector3.one - Vector3.up).magnitude > range)
        {
            state = 1;
        }
        else
        {
            state = 0;
        }
        if(state != previousState)
        {
            time = 0.0f;
        }
        previousState = state;
    }

    void SlowDown()
    {
        transform.position += transform.right * moveSpeed * Time.deltaTime * slowDownCurve.Evaluate(time);
        time += Time.deltaTime;
    }

    void MovingState()
    {
        Vector3 direction = (currentTarget.transform.position - transform.position).normalized;
        float Y = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0.0f, Mathf.LerpAngle(transform.eulerAngles.y, Y - 90.0f, Time.deltaTime * moveRotationSpeed), 0.0f);
        transform.position += transform.right * moveSpeed * Time.deltaTime * speedUpCurve.Evaluate(time);
        time += Time.deltaTime;
    }
}
