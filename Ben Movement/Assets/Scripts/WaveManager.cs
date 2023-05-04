using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public List<float> roundDifficulties = new List<float>();

    public List<EnemySpawnInfo> enemySpawnOptions = new List<EnemySpawnInfo>();
    [SerializeField] float spawnRadius;
    int currentRound;

    [SerializeField] float roundLength;
    float timeSinceRoundStart;

    [SerializeField] int infiniteDifficultyIncrease;
    bool waveRunning;

    [SerializeField] bool startGame;

    private void Start()
    {
        currentRound = 0;
        waveRunning = false;
    }

    private void Update()
    {
        if(waveRunning)
        {
            timeSinceRoundStart += Time.deltaTime;
            if (timeSinceRoundStart >= roundLength)
            {
                SpawnNextRound();
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
                GameObject newEnemy = Instantiate(enemyToSpawn.enemyPrefab, new Vector3(0f, 10f, 0f), Quaternion.identity);
                newEnemy.transform.position = GetSpawnPosition();
            }
        }


    }

    Vector3 GetSpawnPosition()
    {
        float randomAngle = Random.Range(-Mathf.PI, Mathf.PI);
        float x = Mathf.Sin(randomAngle) * spawnRadius;
        float z = Mathf.Cos(randomAngle) * spawnRadius;
        Vector3 spawnPosition = new Vector3(x, 0.5f, z);
        return spawnPosition;
    }

    public void StartGame()
    {
        currentRound = 0;
        SpawnNextRound();
        waveRunning = true;
    }
}
