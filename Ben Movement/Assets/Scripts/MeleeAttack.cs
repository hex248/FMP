using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MeleeAttack
{
    //the overall time the attack takes
    public float attackTime;

    //the distance the attack moves you forwards (can be negative for recoil)
    public float moveDistance;

    //the time you spend moving forwards
    public float moveTime;

    //the time at which you check what you have hit
    public float damageTime;

    public Vector3 hitboxSize;
    public Vector3 hitboxOffset;

    public float force;
    public float damage;

    public SlashVFX vfx;
}
