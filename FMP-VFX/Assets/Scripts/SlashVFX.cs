using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class SlashVFX : MonoBehaviour
{
    [Header("Config")]
    public AttackVFX attackVFX;
    public KeyCode attackKey;
    public bool loop;


    [Header("Info")]
    public bool animating;
    public float timer;

    public float totalTime;
    public float offsetDelta;
    public float currentOffset;
    public float step;

    MeshRenderer mr;
    MeshFilter mf;

    private void Start()
    {
        mr = GetComponent<MeshRenderer>();
        mf = GetComponent<MeshFilter>();
        GetComponent<MeshFilter>().mesh = attackVFX.mesh;

        totalTime = attackVFX.attackDuration + attackVFX.holdDuration + attackVFX.dissolveDuration;
        offsetDelta = attackVFX.targetOffset - attackVFX.startOffset;
        currentOffset = attackVFX.startOffset;
        mr.material.SetVector("_offset", new Vector4(0, currentOffset, 0, 0));
    }

    private void Update()
    {
        mr.material.SetColor("_mainColor", attackVFX.mainColor);
        mr.material.SetColor("_secondaryColor", attackVFX.secondaryColor);
        if (!animating && loop)
        {
            animating = true;
        }
        else if (!animating && Input.GetKeyDown(attackKey))
        {
            animating = true;
        }
        else if (animating)
        {
            // first frame animating?
            if (timer == 0.0f)
            {
                currentOffset = attackVFX.startOffset;
                mr.enabled = true;
            }
            timer += Time.deltaTime;
            // if in attack stage
            if (timer <= attackVFX.attackDuration)
            {
                step = ((offsetDelta / 2) / attackVFX.attackDuration) * Time.deltaTime; // calculate amount to move
            }
            // if in hold stage
            else if (timer > attackVFX.attackDuration && timer <= attackVFX.attackDuration + attackVFX.holdDuration)
            {
                step = 0.0f; // don't move
                // hold!
            }
            // if in dissolve stage
            else if (timer > attackVFX.attackDuration + attackVFX.holdDuration && timer < attackVFX.attackDuration + attackVFX.holdDuration + attackVFX.dissolveDuration)
            {
                step = ((offsetDelta / 2) / attackVFX.dissolveDuration) * Time.deltaTime;
            }
            else
            {
                animating = false;
                //mr.enabled = false;
                timer = 0.0f;
                step = 0.0f;
            }
            currentOffset = currentOffset += step;
            mr.material.SetVector("_offset", new Vector4(0, currentOffset, 0, 0));
        }
    }
}
