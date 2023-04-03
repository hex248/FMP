using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeAnimationScript : MonoBehaviour
{
    public Transform target;
    public float Y;
    void Start()
    {
        
    }

    void LateUpdate()
    {
        //target.localEulerAngles = new Vector3(transform.eulerAngles.x - 90.0f, transform.eulerAngles.y, transform.eulerAngles.z);
        target.rotation = Quaternion.Euler(transform.eulerAngles.x - 90.0f, Y, transform.eulerAngles.z);
    }
}
