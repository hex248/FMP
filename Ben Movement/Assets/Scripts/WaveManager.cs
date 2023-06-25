using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public int currentRound;

    [Header("Spawn Settings")]
    [SerializeField] float spawnRadius;
    public List<float> roundDifficulties = new List<float>();
    public List<string> music = new List<string>();
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

    bool waveRunning;

    [SerializeField] bool startGame;

    PlayerManager playerManager;
    DayNightCycleScript dayNight;

    [Header("Bed Settings")]
    [SerializeField] Bed bed;



    private void Start()
    {
        currentRound = 0;
        waveRunning = false;
        inGrace = true;
        previousEnemySpawnAngle = Mathf.PI;
        playerManager = FindObjectOfType<PlayerManager>();
        dayNight = FindObjectOfType<DayNightCycleScript>();
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
                bed.FullHeal();
                EnterGrace();
            }
        }

        if(startGame)
        {
            startGame = false;
            StartGame();
        }
    }

    [HideInInspector]
    public bool bossRound;

    void SpawnNextRound()
    {
        currentRound++;
        waveRunning = true;
        inGrace = false;
        allEnemiesDead = false;
        dayNight.IsDay(false);

        timeSinceRoundStart = 0f;

        

        if (currentRound == roundDifficulties.Count)
        {
            //boss round
            Debug.Log("boss round!");
            SpawnRoundWithDifficulty(roundDifficulties[currentRound - 1], true);
        }
        else if(currentRound > roundDifficulties.Count)
        {
            //ran out of rounds
        }
        else
        {
            //standard round
            SpawnRoundWithDifficulty(roundDifficulties[currentRound - 1], false);

        }


    }

    IEnumerator ToDayDelay()
    {
        yield return new WaitForSeconds(3f);
        dayNight.IsDay(true);
    }

    void SpawnRoundWithDifficulty(float difficulty, bool includeBosses)
    {
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

            List<EnemySpawnInfo> bossFilteredSpawnOptions = new List<EnemySpawnInfo>();
           
            if (includeBosses)
            {
                //if it is a boss round check if there is a boss option
                bool hasBossOption = false;
                foreach (EnemySpawnInfo enemySpawnInfo in difficultyPossibleSpawnOptions)
                {
                    if (enemySpawnInfo.enemyType == EnemySpawnInfo.EnemyType.Boss)
                    {
                        hasBossOption = true;
                    }
                }
                //if there is, only include the boss options
                if(hasBossOption)
                {
                    foreach (EnemySpawnInfo enemySpawnInfo in difficultyPossibleSpawnOptions)
                    {
                        if (enemySpawnInfo.enemyType == EnemySpawnInfo.EnemyType.Boss)
                        {
                            bossFilteredSpawnOptions.Add(enemySpawnInfo);
                        }
                    }
                }
                //otherwise just use the same list
                else
                {
                    bossFilteredSpawnOptions = difficultyPossibleSpawnOptions;
                }
            }
            //if it is not a boss round filter out all boss options
            else
            {
                foreach (EnemySpawnInfo enemySpawnInfo in difficultyPossibleSpawnOptions)
                {
                    if (enemySpawnInfo.enemyType != EnemySpawnInfo.EnemyType.Boss)
                    {
                        bossFilteredSpawnOptions.Add(enemySpawnInfo);
                    }
                }
            }
            

            //if there are no options to spawn, quit out 
            if(bossFilteredSpawnOptions.Count == 0)
            {
                difficultyLeftToDistribute = 0;
            }
            else
            {
                int randomIndex = Random.Range(0, bossFilteredSpawnOptions.Count);
                EnemySpawnInfo enemyToSpawn = bossFilteredSpawnOptions[randomIndex];
                if(enemyToSpawn.enemyType == EnemySpawnInfo.EnemyType.Regular)
                {
                    difficultyLeftToDistribute -= enemyToSpawn.difficultyCost;

                    enemiesToSpawn.Add(enemyToSpawn);

                    enemiesRemaining++;
                }
                else if(enemyToSpawn.enemyType == EnemySpawnInfo.EnemyType.Boss)
                {
                    difficultyLeftToDistribute = 0.0f;

                    enemiesToSpawn.Add(enemyToSpawn);

                    enemiesRemaining++;
                }
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

        playerManager.RevivePlayers();
        StartCoroutine(ToDayDelay());

        
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
