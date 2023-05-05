using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretProjectile : MonoBehaviour
{
    public float damageAmount = 1;
    public float homingAmount;
    public float maxVelocity;
    [SerializeField] float velocity = 5.0f;
    public AnimationCurve velocityDecay;
    public GameObject target;
    [SerializeField] float lifeTime = 0.0f;
    public float maxLifeTime;

    public Turret parentTurret;

    public LayerMask crashLayers;

    public GameObject crashVFXPrefab;
    public GameObject crashDecalPrefab;

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
        if(lifeTime < maxLifeTime)
        {
            lifeTime += Time.deltaTime;
            //visuals.transform.localScale = Vector3.one * (maxLifeTime - lifeTime) / maxLifeTime;
        }
        else
        {
            Destroy(gameObject);
        }

        if (!locked)
        {
            if (target)
            {
                targetRotation = Quaternion.LookRotation((target.transform.position + new Vector3(0, 1.5f, 0)) - transform.position, Vector3.up);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, homingAmount * Time.deltaTime);
                //transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, 1, Mathf.Infinity), transform.position.z);
            }
            rb.velocity = transform.forward * velocity;
            if (transform.position.y < 1.5f)
            {
                StartCoroutine(Destroy());
            }
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
            Debug.Log("hit target");
            StartCoroutine(Destroy(col));
        }
        // if crashes into environment, and has been active for at least half a second (stops crashing into turret tower straight away)
        if (Crash(col.gameObject.layer) && lifeTime >= 1.5f)
        {
            Debug.Log("hit environment");
            StartCoroutine(Destroy(col));
        }
    }

    IEnumerator Destroy(Collider col=null)
    {
        locked = true;
        GetComponent<Collider>().enabled = false;
        visuals.SetActive(false);

        // notify tower
        parentTurret.ProjectileDestroyed();

        // trigger vfx and decal
        GameObject particleSys = CrashVFX(transform);
        if (col != null)
        {
            var enemyHealth = col.gameObject.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damageAmount, this.gameObject);
            }
        }
        yield return new WaitForSeconds(0.3f);


        // apply decal slightly after explosion starts, to hide it in the explosion
        // only apply to layers marked for crash - mostly environment
        if (col != null && Crash(col.gameObject.layer))
        {
            //CrashDecal(transform, col);
        }

        yield return new WaitForSeconds(2.0f);
        Destroy(gameObject);

        yield return null;
    }

    bool Crash(int layer)
    {
        return crashLayers == (crashLayers | (1 << layer));
    }

    GameObject CrashVFX(Transform impact)
    {
        // vfx
        GameObject spawned = Instantiate(crashVFXPrefab, impact.position, Quaternion.LookRotation(impact.forward * -1));

        return spawned;
    }
    GameObject CrashDecal(Transform impact, Collider crashedCollider)
    {
        // decal

        //GameObject spawned = Instantiate(crashDecalPrefab, impact.position, Quaternion.LookRotation(impact.forward), crashedCollider.transform);
        GameObject spawned = Instantiate(crashDecalPrefab, impact.position, Quaternion.LookRotation(impact.forward));

        return spawned;
    }
}
