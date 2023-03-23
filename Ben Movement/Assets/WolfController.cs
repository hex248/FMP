using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfController : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    Rigidbody rb;
    float timeSinceStart;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceStart += Time.deltaTime;
        Vector3 force = new Vector3(Random.Range(-3f, 3f), 0f, Random.Range(-3f, 3f));
        rb.AddForce(force);
    }
}
