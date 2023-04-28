using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAnimationScript : MonoBehaviour
{
    public float movementSpeedAnimation = 1.0f;
    private Animator anim;
    private Rigidbody rb;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponentInParent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
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
