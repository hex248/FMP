using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class WolfController : MonoBehaviour
{
    // THIS IS A TEMPORARY SCRIPT - BEN THIS IS FOR U TO DELETE!

    public Transform head;
    public VisualEffect smoke;

    public float homingAmount;
    public float maxVelocity;
    [SerializeField] float velocity = 5.0f;
    public AnimationCurve velocityDecay;
    public GameObject target;
    

    Rigidbody rb;

    Quaternion targetRotation;

    public bool locked = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        // TEMP! - DEFINE TARGET PRE GAME
        targetRotation = Quaternion.LookRotation(target.transform.position - head.position, Vector3.up);
        transform.rotation = targetRotation;
    }

    private void Update()
    {
        if (!locked)
        {
            RotateTowardsTarget();
            MoveTowardsTarget();
        }
        else
        {
            rb.velocity = Vector3.zero;
        }
    }

    void RotateTowardsTarget()
    {
        targetRotation = Quaternion.LookRotation(target.transform.position - head.position, Vector3.up);
        head.rotation = Quaternion.Lerp(head.rotation, targetRotation, homingAmount * Time.deltaTime);
        //smoke.SetVector3("EmissionDirection", head.forward * -1);
    }

    void MoveTowardsTarget()
    {
        rb.velocity = head.forward * velocity;
    }
}
