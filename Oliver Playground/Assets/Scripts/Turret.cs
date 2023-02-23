using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("Turret Details")]
    public int level = 1;

    [Header("Settings")]
    public LayerMask enemyLayer;
    public float shootSpeed = 0.5f;
    public float bulletVelocity = 5.0f;
    [Range(0.0f, 360.0f)]
    public float turretHeadRotation;
    public float rotationSpeed = 5.0f;

    [Header("Detection Radius")]
    public float detectionRadius = 10.0f;
    public bool drawDetectionArea = false;
    public int circleVertexCount = 50;
    public Color lineColor = Color.green;
    public LineRenderer detectionRadiusLineRenderer;

    [Header("Components")]
    public GameObject turretHead;

    [Header("Internal Variables")]
    public GameObject target;

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
            if (TargetInRange())
            {
                Debug.Log("rotating towards target");
                RotateTowardsTarget();
            }
            else
            {
                Debug.Log("lost target");
                target = GetClosestTarget();
            }
        }
    }

    void DrawDetectionArea()
    {
        detectionRadiusLineRenderer.positionCount = circleVertexCount + 1;
        detectionRadiusLineRenderer.useWorldSpace = false;
        detectionRadiusLineRenderer.startColor = lineColor;
        detectionRadiusLineRenderer.endColor = lineColor;

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
            Debug.Log(enemy.gameObject.tag);
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

    void RotateTowardsTarget()
    {
        // calculate target rotation with only y axis affected
        Quaternion targetRotation = Quaternion.Euler(turretHead.transform.rotation.eulerAngles.x, Quaternion.LookRotation(target.transform.position - transform.position, Vector3.up).eulerAngles.y + 180, turretHead.transform.rotation.eulerAngles.z);
        turretHead.transform.rotation = Quaternion.Slerp(turretHead.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
