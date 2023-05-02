using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(MeshRenderer))]
public class SlashVFX : MonoBehaviour
{
    public bool isHooves;
    public float attackDuration = 0.0f;
    public Animator hoofAnimation;
    public MeshRenderer[] hooves;

    [Header("Config")]
    public AttackVFX attackVFX;
    public KeyCode attackKey;
    public Light light;
    public float lightIntensity = 2f;
    public float lightRange = 1f;
    public ParticleSystem particleSpawner;
    ParticleSystemRenderer particleRenderer;
    public AnimationCurve curve;
    public float curveOffset;
    public ParticleSystem.MinMaxCurve emissionRate;


    [Header("Info")]
    public bool animating;
    public bool trigger;
    public float timer;

    public float totalTime;
    public float offsetDelta;
    public float currentOffset;
    public float step;

    MeshRenderer mr;
    MeshFilter mf;

    private void Start()
    {
        if (isHooves)
        {
            foreach (MeshRenderer mr in hooves)
            {
                mr.enabled = false;
            }
        }
        else
        {
            mr = GetComponent<MeshRenderer>();
            mf = GetComponent<MeshFilter>();
            GetComponent<MeshFilter>().mesh = attackVFX.mesh;

            particleRenderer = particleSpawner.GetComponent<ParticleSystemRenderer>();

            totalTime = attackVFX.attackDuration + attackVFX.holdDuration + attackVFX.dissolveDuration;
            offsetDelta = attackVFX.targetOffset - attackVFX.startOffset;
            currentOffset = attackVFX.startOffset;
            mr.material.SetVector("_offset", new Vector4(0, currentOffset, 0, 0));

            //particleRenderer.material = mr.material;
        }
    }

    private void Update()
    {
        if (isHooves)
        {
            if (!animating && trigger)
            {
                trigger = false;
                animating = true;

                foreach (MeshRenderer mr in hooves)
                {
                    mr.enabled = true;
                }

                hoofAnimation.SetTrigger("hoof");
            }
            else if (animating)
            {
                timer += Time.deltaTime;

                if (timer >= attackDuration)
                {
                    animating = false;
                    timer = 0.0f;

                    foreach (MeshRenderer mr in hooves)
                    {
                        mr.enabled = false;
                    }
                }
            }
        }
        else
        {
            mr.material.SetColor("_mainColor", attackVFX.mainColor);
            mr.material.SetColor("_secondaryColor", attackVFX.secondaryColor);
            light.color = attackVFX.mainColor;
            particleRenderer.material.SetColor("_Color", attackVFX.secondaryColor);

            //particleRenderer.material.SetColor("_BaseMap", attackVFX.mainColor);
            //particleRenderer.material.SetColor("_EmissionMap", attackVFX.secondaryColor);


            if (!animating && trigger)
            {
                trigger = false;
                animating = true;
            }
            else if (animating)
            {
                // first frame animating?
                if (timer == 0.0f)
                {
                    currentOffset = attackVFX.startOffset;
                    mr.enabled = true;
                    var emission = particleSpawner.emission;
                    emission.rateOverTime = emissionRate;
                    light.intensity = lightIntensity;
                    light.range = lightRange;
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
                    mr.enabled = false;
                    var emission = particleSpawner.emission;
                    emission.rateOverTime = new ParticleSystem.MinMaxCurve(0.0f);
                    timer = 0.0f;
                    step = 0.0f;
                    light.intensity = 0;
                }
                currentOffset = currentOffset += step;
                mr.material.SetVector("_offset", new Vector4(0, currentOffset, 0, 0));
                float progress = ((attackVFX.targetOffset - currentOffset) / offsetDelta);
                progress += curveOffset;
                Vector3 newPos = new Vector3(-1.25f, 0.0f, -1.25f) + new Vector3(curve.Evaluate(progress), 0.0f, progress) * 2.5f;

                particleSpawner.transform.localPosition = newPos;
                light.transform.localPosition = newPos;
            }
        }
    }

    public void StartEffect()
    {
        trigger = true;
    }
}
