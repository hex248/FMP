using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("Turret Details")]
    public int level = 1;

    [Header("Settings")]
    public LayerMask enemyLayer;
    public Transform projectileParent;
    public float projectileHomingAmount = 1.0f;
    public float projectileMaxVelocity = 5.0f;
    public float projectileVelocity = 5.0f;
    public float projectileLifetime = 5.0f;
    public AnimationCurve projectileDecay;
    public GameObject projectilePrefab;
    public LayerMask crashLayer;

    [Header("Detection Radius")]
    public float detectionRadius = 10.0f;
    public bool drawDetectionArea = false;
    public int circleVertexCount = 50;
    public Color lineColor = Color.green;
    public LineRenderer detectionRadiusLineRenderer;

    [Header("Components")]
    public GameObject turretOrb;

    [Header("Internal Variables")]
    public GameObject target;
    bool canShoot = true;

    void Update()
    {
        if (drawDetectionArea)
        {
            DrawDetectionArea();
        }

        if (target == null)
        {
            Debug.Log("getting closest target");
            target = GetClosestTarget();
        }
        else
        {
            if (TargetInRange() && canShoot)
            {
                canShoot = false;
                GameObject spawnedObj = Instantiate(projectilePrefab, turretOrb.transform.position, Quaternion.identity, projectileParent);
                Projectile spawnedProjectile = spawnedObj.GetComponent<Projectile>();
                spawnedProjectile.homingAmount = projectileHomingAmount;
                spawnedProjectile.maxVelocity = projectileMaxVelocity;
                spawnedProjectile.velocityDecay = projectileDecay;
                spawnedProjectile.target = target;
                spawnedProjectile.maxLifeTime = projectileLifetime;
                spawnedProjectile.parentTurret = this;
                spawnedProjectile.crashLayer = crashLayer;
            }
            else
            {
                Debug.Log("lost target");
                target = GetClosestTarget();
            }
        }
    }

    public void ProjectileDestroyed()
    {
        canShoot = true;
    }

    void DrawDetectionArea()
    {
        detectionRadiusLineRenderer.positionCount = circleVertexCount + 1;
        detectionRadiusLineRenderer.useWorldSpace = false;
        detectionRadiusLineRenderer.material.color = lineColor;

        float x;
        float y;
        float z;

        float angle = 20f;

        for (int i = 0; i < (circleVertexCount + 1); i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * detectionRadius;
            z = Mathf.Cos(Mathf.Deg2Rad * angle) * detectionRadius;

            detectionRadiusLineRenderer.SetPosition(i, new Vector3(x, 0, z));

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
}
