using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[ExecuteAlways]
[RequireComponent(typeof(MeshRenderer))]
public class SlashVFX : MonoBehaviour
{
    [Header("Config")]
    public AttackVFX attackVFX;
    public KeyCode attackKey;
    public bool loop;
    public VisualEffect particles;
    public Transform particleSpawner;
    public AnimationCurve curve;
    public float curveOffset;


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
        mr.sharedMaterial.SetVector("_offset", new Vector4(0, currentOffset, 0, 0));
    }

    private void Update()
    {
        mr.sharedMaterial.SetColor("_mainColor", attackVFX.mainColor);
        mr.sharedMaterial.SetColor("_secondaryColor", attackVFX.secondaryColor);
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
            mr.sharedMaterial.SetVector("_offset", new Vector4(0, currentOffset, 0, 0));
            float progress = ((attackVFX.targetOffset - currentOffset) / offsetDelta);
            progress += curveOffset;
            Vector3 newPos = new Vector3(-1.25f, 0.0f, -1.25f) + new Vector3(curve.Evaluate(progress), 0.0f, progress) * 2.5f;
            particles.SetVector3("SpawnPosition", newPos);

            particleSpawner.localPosition = newPos;
        }
    }
}
