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
            health = value;
            UpdateHealthBar();
        }
    }

    [SerializeField] int healthTest;

    Transform[] currentHitPointTransforms;
    List<GameObject> currentHitPointObjects = new List<GameObject>();
    [SerializeField] GameObject hitPointPrefab;

    private void Update()
    {
        Health = healthTest;
    }

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

        int loopLength = Mathf.Max(currentHitPointObjects.Count, health);
        for(int i = 0; i < loopLength; i++)
        {
            if(i < currentHitPointObjects.Count)
            {
                if(i < health)
                {
                    //keep object
                }

                else
                {
                    Destroy(currentHitPointObjects[i]);
                    currentHitPointObjects.Remove(currentHitPointObjects[i]);
                }

            }
            else if(i < health)
            {
                GameObject newHP = Instantiate(hitPointPrefab);
                newHP.transform.SetParent(transform);
                currentHitPointObjects.Add(newHP);
            }
        }


    }

    private void Start()
    {
        
    }
}
