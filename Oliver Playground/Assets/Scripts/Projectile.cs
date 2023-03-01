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

    public GameObject crashVFXPrefab;

    public GameObject visuals;

    Rigidbody rb;

    Quaternion targetRotation;

    bool locked = false;

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

        if (!locked)
        {
            targetRotation = Quaternion.LookRotation(target.transform.position - transform.position, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, homingAmount * Time.deltaTime);
            rb.velocity = transform.forward * velocity;
        }
        else
        {
            rb.velocity = Vector3.zero;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.GetInstanceID() == target.GetInstanceID())
        {
            StartCoroutine(Destroy());
        }
        // if crashes into environment, and has been active for at least half a second (stops crashing into turret tower straight away
        if (Crash(col.gameObject.layer) && lifeTime >= 1.5f)
        {
            StartCoroutine(Destroy());
        }
    }

    IEnumerator Destroy()
    {
        locked = true;
        GetComponent<Collider>().enabled = false;
        visuals.SetActive(false);

        // notify tower
        parentTurret.ProjectileDestroyed();

        // trigger vfx
        CrashDecal();
        GameObject particleSys = CrashVFX(transform);

        yield return new WaitForSeconds(2.0f);
        
        Destroy(particleSys);

        // destroy gameObject

        Destroy(gameObject);

        yield return null;
    }

    bool Crash(int layer)
    {
        return crashLayer == (crashLayer | (1 << layer));
    }

    GameObject CrashVFX(Transform impact)
    {
        // vfx
        GameObject spawned = Instantiate(crashVFXPrefab, impact.position, Quaternion.LookRotation(impact.forward * -1));

        return spawned;
    }
    void CrashDecal()
    {
        // decal



    }
}
