using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemySpawnInfo
{
    public GameObject enemyPrefab;
    public int minRoundNumberToSpawn;
    public EnemyType enemyType;
    public enum EnemyType { Regular, Boss };
    //the amount of value that the enemy takes up. Bigger = Harder
    public float difficultyCost;

}
