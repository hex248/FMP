using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Scriptables/AttackVFX")]
public class AttackVFX : ScriptableObject
{
    public string name;
    public Mesh mesh;
    public Material material;
    public float attackDuration;
    public float holdDuration;
    public float dissolveDuration;
    public float startOffset;
    public float targetOffset;
    [ColorUsage(true, true)]
    public Color[] mainColors;
    [ColorUsage(true, true)]
    public Color[] secondaryColors;
}
