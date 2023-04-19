using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeController : MonoBehaviour
{
    public Transform target;
    public Transform rotationReference;
    [SerializeField] private float moveSpeed = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.Scale(rotationReference.rotation * transform.right, Vector3.one - Vector3.up) * Time.deltaTime * moveSpeed;
    }
}
