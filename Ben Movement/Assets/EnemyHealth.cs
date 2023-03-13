using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] float maximumHealth;
    [SerializeField] float currentHealth;
    float healthFactor;

    MeshRenderer skinnedMesh;
    Material[] skinnedMaterials;

    //limits the amount u can dissolve until you die
    [SerializeField, Range(0f, 1f)] float maxDamageDissolveAmount;
    [SerializeField] float deathDissolveRate = 0.00001f;
    float targetDissolveFactor;

    public bool triggerDeath = false;


    void Start()
    {
        currentHealth = maximumHealth;

        skinnedMesh = GetComponent<MeshRenderer>();
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
        healthFactor = 1 - currentHealth / maximumHealth;
        targetDissolveFactor = healthFactor * maxDamageDissolveAmount;
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
            targetDissolveFactor = healthFactor * maxDamageDissolveAmount;
            UpdateDissolve();
        }
    }

    public void Die()
    {
        StartCoroutine(DeathDissolve());

    }

    void UpdateDissolve()
    {
        
        for (int i = 0; i < skinnedMaterials.Length; i++)
        {
            skinnedMaterials[i].SetFloat("_DissolveAmount", targetDissolveFactor);
        }
    }




    // Update is called once per frame


    IEnumerator DeathDissolve()
    {
        if (skinnedMaterials.Length > 0)
        {
            while (targetDissolveFactor < 1)
            {
                targetDissolveFactor += deathDissolveRate;
                UpdateDissolve();
                Debug.Log(targetDissolveFactor);
                yield return new WaitForSeconds(0.02f);
            }
        }

        Destroy(gameObject);
    }
}
