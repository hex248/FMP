using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Spawning Options")]
    public bool spawnEnemies;
    public float spawnRadius;
    float previousSpawnRadius;
    public int spawnAmount;
    public float spawnFrequency;
    public Transform enemiesParent;

    float timer = 0.0f;

    [Header("Enemy")]
    public GameObject enemyPrefab;
    public Transform target;


    [Header("Debug")]
    public bool drawSpawnArea;
    public int circleVertexCount;
    public Color lineColor;
    public LineRenderer spawnAreaLineRenderer;

    public List<Vector3> spawnPositions = new List<Vector3>();

    bool firstPass = true;


    void Start()
    {
        
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (drawSpawnArea)
        {
            DrawSpawnArea();
        }

        if (spawnEnemies)
        {
            // check if it is time to spawn enemies
            if (timer >= spawnFrequency)
            {
                // reset timer
                timer = 0.0f;
                // spawn enemies
                SpawnEnemies(spawnAmount);
            }
        }
    }

    void DrawSpawnArea()
    {
        spawnAreaLineRenderer.positionCount = circleVertexCount + 1;
        spawnAreaLineRenderer.useWorldSpace = false;
        spawnAreaLineRenderer.material.color = lineColor;

        float x;
        float y;
        float z;

        float angle = 20f;

        if (firstPass || spawnRadius != previousSpawnRadius)
        {
            spawnPositions.Clear();
        }
        for (int i = 0; i < (circleVertexCount + 1); i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * spawnRadius;
            z = Mathf.Cos(Mathf.Deg2Rad * angle) * spawnRadius;
            Vector3 vertexPosition = new Vector3(x, 0, z);
            if (firstPass || spawnRadius != previousSpawnRadius)
            {
                spawnPositions.Add(vertexPosition);
            }

            spawnAreaLineRenderer.SetPosition(i, vertexPosition);

            angle += (360f / circleVertexCount);
        }
        previousSpawnRadius = spawnRadius;
        firstPass = false;
    }

    public void SpawnEnemies(int spawnAmount)
    {
        // randomise spawnPositions
        System.Random random = new System.Random();
        List<Vector3> placesToSpawn = spawnPositions.OrderBy(x => random.Next()).Take(spawnAmount).ToList();

        foreach(Vector3 spawnPosition in placesToSpawn)
        {
            SpawnEnemy(spawnPosition);
        }
    }

    public void SpawnEnemy(Vector3 position)
    {
        GameObject spawnedObject = Instantiate(enemyPrefab, position, Quaternion.LookRotation(target.position - position), enemiesParent);

        Enemy enemy = spawnedObject.GetComponentInChildren<Enemy>();
        enemy.target = target;
    }
}
