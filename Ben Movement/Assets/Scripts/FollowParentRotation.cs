using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowParentRotation : MonoBehaviour
{
    public Transform parent;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3(transform.eulerAngles.x, parent.eulerAngles.y, transform.eulerAngles.z)), Time.deltaTime);
    }
}
