using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthBar : MonoBehaviour
{
    int health;
    
    public int Health
    {
        get
        {
            return health;
        }
        set
        {
            if(value != health)
            {
                health = value;
                UpdateHealthBar();
            }
        }
    }

    [SerializeField] int healthTest;

    Transform[] currentHitPointTransforms;
    List<GameObject> currentHitPointObjects = new List<GameObject>();
    [SerializeField] GameObject hitPointPrefab;

    Player player;
    PlayerHealth playerHealth;

    List<GameObject> toRemove = new List<GameObject>();


    void UpdateHealthBar()
    {
        /*if(currentHitPointObjects.Count == 0)
        {
            currentHitPointObjects = new List<GameObject>();
            currentHitPointTransforms = GetComponentsInChildren<Transform>();
            foreach(Transform loopTransform in currentHitPointTransforms)
            {
                currentHitPointObjects.Add(loopTransform.gameObject);
            }
        }*/
        toRemove = new List<GameObject>();
        int loopLength = Mathf.Max(currentHitPointObjects.Count, Health);
        for(int i = 0; i < loopLength; i++)
        {
            if(i < currentHitPointObjects.Count)
            {
                if(i < Health)
                {
                    //keep object
                }

                else
                {
                    Destroy(currentHitPointObjects[i]);
                    toRemove.Add(currentHitPointObjects[i]);
                }

            }
            else if(i < Health)
            {
                GameObject newHP = Instantiate(hitPointPrefab);
                newHP.transform.SetParent(transform);
                currentHitPointObjects.Add(newHP);
            }
        }

        foreach(GameObject remove in toRemove)
        {
            currentHitPointObjects.Remove(remove);
        }


    }


    public void Init(Player inPlayer)
    {
        playerHealth = inPlayer.playerHealth;
        Health = playerHealth.GetCurrentHitPoints();
    }
}
