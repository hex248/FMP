using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeAnimationScript : MonoBehaviour
{
    public Transform parent;
    public Transform target;
    [Header("Movement")]
    [SerializeField] private float turnSpeed = 1.0f;
    [SerializeField] private float wiggleSpeed = 2.0f;
    [SerializeField] private float wiggleAmplitude = 10.0f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 pointToTarget = (transform.position - target.position);
        parent.rotation = Quaternion.Lerp(parent.rotation, Quaternion.Euler(new Vector3(parent.eulerAngles.x, Mathf.Atan2(pointToTarget.z, -pointToTarget.x) * Mathf.Rad2Deg + (Mathf.Sin(Time.time * wiggleSpeed) * wiggleAmplitude), parent.eulerAngles.z)), Time.deltaTime * turnSpeed);
    }
}
