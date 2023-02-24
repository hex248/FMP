using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float homingAmount;
    public float maxVelocity;
    [SerializeField] float velocity = 5.0f;
    public AnimationCurve velocityDecay;
    public GameObject target;
    [SerializeField] float lifeTime = 0.0f;
    public float maxLifeTime;

    public Turret parentTurret;

    public LayerMask crashLayer;

    Rigidbody rb;

    Quaternion targetRotation;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        targetRotation = Quaternion.LookRotation(target.transform.position - transform.position, Vector3.up);
        transform.rotation = targetRotation;
    }

    private void Update()
    {
        lifeTime += Time.deltaTime;
        //velocity = maxVelocity * velocityDecay.Evaluate(lifeTime / maxLifeTime);

        targetRotation = Quaternion.LookRotation(target.transform.position - transform.position, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, homingAmount * Time.deltaTime);
        rb.velocity = transform.forward * velocity;
    }

    void OnTriggerEnter(Collider col)
    {
        Debug.Log(Crash(col.gameObject.layer));
        if (col.gameObject.GetInstanceID() == target.GetInstanceID() || Crash(col.gameObject.layer))
        {
            StartCoroutine(Destroy());
        }
    }

    IEnumerator Destroy()
    {
        // notify tower
        parentTurret.ProjectileDestroyed();

        // trigger vfx

        //yield return new WaitForSeconds(0.5f);

        // destroy gameObject
        Destroy(gameObject);

        yield return null;
    }

    bool Crash(int layer)
    {
        return crashLayer == (crashLayer | (1 << layer));
    }
}
