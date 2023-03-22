using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoovesVFX : MonoBehaviour
{
    [Header("Config")]
    public AttackVFX attackVFX;
    public KeyCode attackKey;
    public bool loop;
    //public ParticleSystem particleSpawner;
    //ParticleSystemRenderer particleRenderer;
    public AnimationCurve movementCurve;
    public float curveOffset;
    public float scale;
    public GameObject leftHoof;
    public GameObject rightHoof;

    //public ParticleSystem.MinMaxCurve emissionRate;

    [Header("Info")]
    public bool animating;
    public float timer;

    public float totalTime;
    public float offsetDelta;
    public float currentOffset;
    public float step;
    public float progress;

    //MeshRenderer mr;
    //MeshFilter mf;

    private void Start()
    {
        //mr = GetComponent<MeshRenderer>();
        //mf = GetComponent<MeshFilter>();
        //GetComponent<MeshFilter>().mesh = attackVFX.mesh;

        //particleRenderer = particleSpawner.GetComponent<ParticleSystemRenderer>();

        totalTime = attackVFX.attackDuration + attackVFX.holdDuration + attackVFX.dissolveDuration;
        offsetDelta = attackVFX.targetOffset - attackVFX.startOffset;
        currentOffset = attackVFX.startOffset;
        //mr.material.SetVector("_offset", new Vector4(0, currentOffset, 0, 0));

        //particleRenderer.material = mr.material;
    }

    private void Update()
    {

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
            timer += Time.deltaTime;
            if (timer <= attackVFX.attackDuration)
            {
                progress = timer / attackVFX.attackDuration;
                
                Vector3 newPos = new Vector3(-1.25f, 0.0f, -1.25f) + new Vector3(movementCurve.Evaluate(progress), 0.0f, progress) * scale;

                leftHoof.transform.position = newPos;
                rightHoof.transform.position = new Vector3(newPos.x * -1, newPos.y, newPos.z);
            }
            else
            {
                animating = false;
                //mr.enabled = false;
                //var emission = particleSpawner.emission;
                //emission.rateOverTime = new ParticleSystem.MinMaxCurve(0.0f);
                timer = 0.0f;
                step = 0.0f;
            }
        }
    }
}
