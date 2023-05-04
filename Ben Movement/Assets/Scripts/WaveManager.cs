using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    

    
    [SerializeField] float spawnRadius;
    int currentRound;

    [Header("Spawn Settings")]

    public List<float> roundDifficulties = new List<float>();
    public List<EnemySpawnInfo> enemySpawnOptions = new List<EnemySpawnInfo>();
    List<EnemySpawnInfo> enemiesToSpawn = new List<EnemySpawnInfo>();
    [SerializeField] float maximumTimeBetweenSpawns = 1f;
    [SerializeField] float spawnPeriodLength = 5f;
    [SerializeField, Range(0f, 1f)] float clusterSpawnChance;
    [SerializeField] float clusterSpawnOffset;

    float previousEnemySpawnAngle;
    

    [Header("Wave Settings")]
    float timeSinceRoundStart;
    bool allEnemiesDead;
    int enemiesRemaining;

    [SerializeField] float gracePeriodLength;
    float timeSinceGraceStart;
    bool inGrace = true;

    [SerializeField] int infiniteDifficultyIncrease;
    bool waveRunning;

    [SerializeField] bool startGame;

    

    private void Start()
    {
        currentRound = 0;
        waveRunning = false;
        inGrace = true;
        previousEnemySpawnAngle = Mathf.PI;
    }

    private void Update()
    {
        if(inGrace)
        {
            timeSinceGraceStart += Time.deltaTime;
            if(timeSinceGraceStart >= gracePeriodLength)
            {
                SpawnNextRound();
            }
        }
        else if(waveRunning)
        {
            timeSinceRoundStart += Time.deltaTime;
            if (allEnemiesDead)
            {
                EnterGrace();
            }
        }

        if(startGame)
        {
            startGame = false;
            StartGame();
        }
    }

    

    void SpawnNextRound()
    {
        currentRound++;
        waveRunning = true;
        inGrace = false;
        allEnemiesDead = false;

        timeSinceRoundStart = 0f;
        if (currentRound >= roundDifficulties.Count)
        {
            //no more hard coded rounds - increase by infinite difficulty increase and continue
            float difficulty = roundDifficulties[roundDifficulties.Count - 1] + (currentRound - roundDifficulties.Count) * infiniteDifficultyIncrease;
            SpawnRoundWithDifficulty(difficulty);
        }
        SpawnRoundWithDifficulty(roundDifficulties[currentRound - 1]);
    }

    void SpawnRoundWithDifficulty(float difficulty)
    {
        Debug.Log("Round " + currentRound + " has difficulty " + difficulty);
        float difficultyLeftToDistribute = difficulty;

        enemiesToSpawn = new List<EnemySpawnInfo>();


        while(difficultyLeftToDistribute > 0)
        {
            List<EnemySpawnInfo> difficultyPossibleSpawnOptions = new List<EnemySpawnInfo>();
            foreach(EnemySpawnInfo enemySpawnInfo in enemySpawnOptions)
            {
                //check if it is a high enough round
                if (enemySpawnInfo.minRoundNumberToSpawn <= currentRound)
                {
                    if(enemySpawnInfo.difficultyCost <= difficultyLeftToDistribute)
                    {
                        difficultyPossibleSpawnOptions.Add(enemySpawnInfo);
                    }
                }
            }

            //if there are no options to spawn, quit out 
            if(difficultyPossibleSpawnOptions.Count == 0)
            {
                difficultyLeftToDistribute = 0;
            }
            else
            {
                int randomIndex = Random.Range(0, difficultyPossibleSpawnOptions.Count);
                EnemySpawnInfo enemyToSpawn = difficultyPossibleSpawnOptions[randomIndex];
                difficultyLeftToDistribute -= enemyToSpawn.difficultyCost;

                enemiesToSpawn.Add(enemyToSpawn);
                
                enemiesRemaining++;
            }
        }

        StartCoroutine(EnemySpawnCoroutine());


    }

    IEnumerator EnemySpawnCoroutine()
    {
        float delayBetweenSpawns = Mathf.Min(maximumTimeBetweenSpawns, (spawnPeriodLength / enemiesToSpawn.Count));
        foreach(EnemySpawnInfo enemy in enemiesToSpawn)
        {
            GameObject newEnemy = Instantiate(enemy.enemyPrefab, new Vector3(0f, 10f, 0f), Quaternion.identity);
            newEnemy.transform.position = GetSpawnPosition();
            newEnemy.GetComponent<EnemyHealth>().LinkSpawner(this);

            yield return new WaitForSeconds(delayBetweenSpawns);
        }
    }

    Vector3 GetSpawnPosition()
    {
        float angle = Mathf.PI;
        if (Random.Range(0f, 1f) < clusterSpawnChance)
        {
            //use same spawn point slightly offset
            angle += Mathf.PI * Random.Range(-clusterSpawnOffset, clusterSpawnOffset);
        }
        else
        {
            //choose completely new spawn point
            angle = Random.Range(-Mathf.PI, Mathf.PI);
        }

        float x = Mathf.Sin(angle) * spawnRadius;
        float z = Mathf.Cos(angle) * spawnRadius;
        Vector3 spawnPosition = new Vector3(x, 0.5f, z);
        return spawnPosition;

        previousEnemySpawnAngle = angle;
    }

    public void StartGame()
    {
        currentRound = 0;
        SpawnNextRound();
        waveRunning = true;
    }

    void EnterGrace()
    {
        timeSinceGraceStart = 0f;
        waveRunning = false;
        inGrace = true;
    }

    public void EnemyDeath()
    {
        enemiesRemaining--;
        if(enemiesRemaining <= 0)
        {
            allEnemiesDead = true;
        }
    }

}
