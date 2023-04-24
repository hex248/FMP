using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] int maxHitPoints = 5;
    int currentHitPoints;
    [SerializeField] float invincibilityTime;
    float timeSinceHit;
    bool invincible;

    PlayerController controller;
    PlayerAnimationScript anim;


    private void Start()
    {
        currentHitPoints = maxHitPoints;
        controller = GetComponent<PlayerController>();
        anim = GetComponentInChildren<PlayerAnimationScript>();
    }

    private void Update()
    {
        if(invincible)
        {
            timeSinceHit += Time.deltaTime;
            if(timeSinceHit >= invincibilityTime)
            {
                invincible = false;
            }
        }
      

    }

    public void Damage(int amount = 1)
    {
        if (invincible)
            return;

        currentHitPoints -= amount;

        currentHitPoints = Mathf.Clamp(currentHitPoints, 0, maxHitPoints);

        timeSinceHit = 0f;
        invincible = true;
        anim.Damage();
        controller.Damage();
    }

    void Die()
    {
        //death logic
    }

    public void Heal(int amount = 1, bool overHeal = false)
    {
        currentHitPoints += amount;

        if(overHeal)
        {
            currentHitPoints = Mathf.Max(0, currentHitPoints);
        }
        else
        {
            currentHitPoints = Mathf.Clamp(currentHitPoints, 0, maxHitPoints);
        }
    }
}
