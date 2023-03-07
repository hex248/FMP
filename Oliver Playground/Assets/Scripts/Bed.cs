using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bed : MonoBehaviour
{
    public int startHealth;
    public int health;
    public Renderer mesh;
    GameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    public int TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            gameManager.GameOver();
        }


        return health;
    }
}
