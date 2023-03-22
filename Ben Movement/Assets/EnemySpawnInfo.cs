using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemySpawnInfo
{
    public GameObject enemyPrefab;
    public int minRoundNumberToSpawn;

    //the amount of value that the enemy takes up. Bigger = Harder
    public float difficultyCost;

}
