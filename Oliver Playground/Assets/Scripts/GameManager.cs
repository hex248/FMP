using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int bedHealth;

    public Bed bed;

    private void Start()
    {
        bed.startHealth = bedHealth;
        bed.health = bedHealth;
    }

    public void GameOver()
    {

    }
}
