using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] float maximumHealth;
    [SerializeField] float currentHealth;
    float healthFactor;

    SkinnedMeshRenderer skinnedMesh;
    Material[] skinnedMaterials;

    //limits the amount u can dissolve until you die
    [SerializeField] bool dissolveActive = true;
    [SerializeField, Range(0f, 1f)] float maxDamageDissolveAmount;
    [SerializeField] float deathDissolveRate = 0.00001f;
    [SerializeField] float maxTimeToTargetDissove = 1f;

    float targetDissolveAmount;
    float currentDissolveAmount;

    float timeSinceTargetAdjusted;
    float dissolveDistance;
    float dissolveSmoothFac;

    public bool triggerDeath = false;


    void Start()
    {
        currentHealth = maximumHealth;

        skinnedMesh = GetComponentInChildren<SkinnedMeshRenderer>();
        if (skinnedMesh != null)
            skinnedMaterials = skinnedMesh.materials;
    }

    private void Update()
    {
        if (triggerDeath)
        {
            triggerDeath = false;
            Die();
        }
        UpdateDissolve();
    }

    public void TakeDamage(float damage)
    {
        damage = Mathf.Max(damage, 0f);
        currentHealth -= damage;

        currentHealth = Mathf.Clamp(currentHealth, 0f, maximumHealth);

        if (currentHealth == 0f)
        {
            Die();
        }
        else
        {
            healthFactor = 1 - currentHealth / maximumHealth;

            targetDissolveAmount = healthFactor * maxDamageDissolveAmount;
            dissolveDistance = Mathf.Abs(currentDissolveAmount - targetDissolveAmount);

            timeSinceTargetAdjusted = 0f;

        }
    }

    public void Die()
    {
        if (dissolveActive)
            StartCoroutine(DeathDissolve());

    }

    void UpdateDissolve()
    {
        if(dissolveActive)
        {
            timeSinceTargetAdjusted += Time.deltaTime;
            dissolveSmoothFac = Mathf.Min(1f, timeSinceTargetAdjusted / 1f);

            currentDissolveAmount = Mathf.SmoothStep(currentDissolveAmount, targetDissolveAmount, dissolveSmoothFac);
            for (int i = 0; i < skinnedMaterials.Length; i++)
            {
                skinnedMaterials[i].SetFloat("_DissolveAmount", currentDissolveAmount);
            }
        }
    }
        




    // Update is called once per frame


    IEnumerator DeathDissolve()
    {
        if (skinnedMaterials.Length > 0)
        {
            while (targetDissolveAmount < 1)
            {
                targetDissolveAmount += deathDissolveRate;

                Debug.Log(targetDissolveAmount);
                yield return new WaitForSeconds(0.02f);
            }
        }

        Destroy(gameObject);
    }
}
