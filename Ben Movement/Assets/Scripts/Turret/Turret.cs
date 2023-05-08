using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    AudioManager AM;


    [Header("Turret Details")]
    public int level = 1;

    [Header("Settings")]
    Vector3 turretPos;
    public LayerMask enemyLayer;
    public float projectileHomingAmount = 1.0f;
    public float projectileMaxVelocity = 5.0f;
    public float projectileVelocity = 5.0f;
    public float projectileLifetime = 5.0f;
    public float shootBufferTime = 0.35f;
    public int damage;
    public AnimationCurve projectileDecay;
    public GameObject projectilePrefab;
    public LayerMask crashLayer;
    public float riseSpeed = 2.5f;
    
    [Header("Detection Radius")]
    public float detectionRadius = 10.0f;
    public bool drawDetectionArea = false;
    public int circleVertexCount = 50;
    public float lineWidth = 5.0f;
    public Color lineColor = Color.green;
    public LineRenderer detectionRadiusLineRenderer;

    [Header("Components")]
    public GameObject projectileSpawn;

    [Header("Internal Variables")]
    public GameObject target;
    bool canShoot = true;

    [Header("Visuals")]
    public GameObject[] lights;
    public GameObject visuals;
    public Material lineRendererMat;

    private void Start()
    {
        AM = FindObjectOfType<AudioManager>();
        turretPos = visuals.transform.position;
        StartCoroutine(SpawnTurret());
    }

    void Update()
    {
        if (drawDetectionArea)
        {
            DrawDetectionArea();
        }

        if (target == null)
        {
            //Debug.Log("getting closest target");
            target = GetClosestTarget();
        }
        else
        {
            if (TargetInRange() && canShoot)
            {
                canShoot = false;
                if (AM.turretShootOn)
                {
                    AM.PlayInChannel($"turret_shooting", ChannelType.SFX, 2);
                }
                GameObject spawnedObj = Instantiate(projectilePrefab, projectileSpawn.transform.position, Quaternion.identity);
                TurretProjectile spawnedProjectile = spawnedObj.GetComponent<TurretProjectile>();
                spawnedProjectile.damage = damage;
                spawnedProjectile.homingAmount = projectileHomingAmount;
                spawnedProjectile.maxVelocity = projectileMaxVelocity;
                spawnedProjectile.velocityDecay = projectileDecay;
                spawnedProjectile.target = target;
                spawnedProjectile.maxLifeTime = projectileLifetime;
                spawnedProjectile.parentTurret = this;
                spawnedProjectile.crashLayers = crashLayer;
            }
            else
            {
                //Debug.Log("lost target");
                target = GetClosestTarget();
            }
        }
    }

    public void ProjectileDestroyed()
    {
        StartCoroutine(ShootBuffer());

        if (AM.turretExplodeOn)
        {
            AM.PlayInChannel($"turret_projectile-explosion", ChannelType.SFX, 2);
        }
    }

    void DrawDetectionArea()
    {
        detectionRadiusLineRenderer.positionCount = circleVertexCount + 1;
        //detectionRadiusLineRenderer.useWorldSpace = false;
        detectionRadiusLineRenderer.material.SetColor("_Color", lineColor);
        detectionRadiusLineRenderer.startWidth = lineWidth;
        detectionRadiusLineRenderer.endWidth = lineWidth;

        float x;
        float y;
        float z;

        float angle = 20f;

        for (int i = 0; i < (circleVertexCount + 1); i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * detectionRadius;
            z = Mathf.Cos(Mathf.Deg2Rad * angle) * detectionRadius;

            detectionRadiusLineRenderer.SetPosition(i, detectionRadiusLineRenderer.transform.position + new Vector3(x, 0, z));

            angle += (360f / circleVertexCount);
        }
    }

    bool TargetInRange()
    {
        return Vector3.Distance(transform.position, target.transform.position) <= detectionRadius;
    }

    GameObject GetClosestTarget()
    {
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);
        GameObject closestTarget = null;
        foreach (Collider enemy in enemiesInRange)
        {
            if (closestTarget != null)
            {
                // if this enemy is closer than the previous closest target
                if (Vector3.Distance(transform.position, enemy.gameObject.transform.position) < Vector3.Distance(transform.position, closestTarget.transform.position))
                {
                    closestTarget = enemy.gameObject;
                }
            }
            else closestTarget = enemy.gameObject;
        }

        return closestTarget;
    }

    IEnumerator SpawnTurret()
    {
        visuals.transform.position = new Vector3(turretPos.x, -10.0f, turretPos.z);
        foreach (GameObject light in lights)
        {
            //light.SetActive(false);
        }

        
        float distanceToMove = (turretPos - visuals.transform.position).sqrMagnitude;
        float dissolveSpeed = 1.5f;
        bool fadingLR = false;

        detectionRadiusLineRenderer.material = lineRendererMat;

        for (;;)
        {
            // move towards turretPos

            visuals.transform.position = Vector3.Lerp(visuals.transform.position, turretPos, riseSpeed * Time.deltaTime);
            if (fadingLR)
            {
                float currentDissolve = detectionRadiusLineRenderer.material.GetFloat("_Alpha");
                detectionRadiusLineRenderer.material.SetFloat("_Alpha", Mathf.Lerp(currentDissolve, 1.0f, dissolveSpeed * Time.deltaTime));
            }
            else
            {
                detectionRadiusLineRenderer.material.SetFloat("_Alpha", 0.0f);
            }

            // if movement is 3/4
            if ((turretPos - visuals.transform.position).sqrMagnitude <= distanceToMove/4)
            {
                // start undissolving line renderer
                fadingLR = true;
            }
            if (visuals.transform.position == turretPos)
            {
                break;
            }

            yield return new WaitForEndOfFrame();
        }

        foreach (GameObject light in lights)
        {
            //light.SetActive(true);
        }
    }

    IEnumerator ShootBuffer()
    {
        yield return new WaitForSeconds(shootBufferTime);
        canShoot = true;
    }
}
