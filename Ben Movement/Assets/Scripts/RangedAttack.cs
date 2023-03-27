using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RangedAttack
{
    public GameObject projectile;
    //the overall time the attack takes
    public float attackTime;

    //the distance the attack moves you forwards (can be negative for recoil)
    public float moveDistance;

    //the time you spend moving forwards
    public float moveTime;

    //the time at which you check what you have hit
    public float projectileSpawnTime;

    public Vector3 projectileSpawnOffset;

    public float force;
    public float damage;
}