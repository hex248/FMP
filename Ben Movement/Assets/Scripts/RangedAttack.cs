using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RangedAttack
{
    public GameObject projectile;
    [Header("Charging Settings")]
    public float fullChargeTime;
    public float fullChargeScaleFactor;
    public float fullChargeDamageMultiplier;
    public float fullChargeForceMultiplier;
    public float fullChargeSpeedMultiplier;


    [Header("Attack Settings")]
    
    //the overall time the attack takes
    public float attackTime;

    //the distance the attack moves you forwards (can be negative for recoil)
    public float moveDistance;

    //the time you spend moving forwards
    public float moveTime;

    //the time at which you check what you have hit
    public float projectileSpawnTime;

    public Transform projectileSpawnPosition;

    public float baseForce;
    public float baseDamage;
}