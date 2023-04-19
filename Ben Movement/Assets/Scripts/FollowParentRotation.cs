using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowParentRotation : MonoBehaviour
{
    public Transform parent;
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
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3(transform.eulerAngles.x, parent.eulerAngles.y + Mathf.Sin(Time.time * wiggleSpeed) * wiggleAmplitude, transform.eulerAngles.z)), Time.smoothDeltaTime * turnSpeed);
    }
}
