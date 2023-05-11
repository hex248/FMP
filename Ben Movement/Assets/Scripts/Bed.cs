using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

public class Bed : MonoBehaviour
{
    [SerializeField] float maximumHealth;
    [SerializeField] float currentHealth;
    float healthFactor;

    List<MeshRenderer> skinnedMeshRenderers;
    List<Material> skinnedMaterials = new List<Material>();

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

    [SerializeField] UnityEvent<GameObject> damageEvent;
    [SerializeField] UnityEvent deathEvent;

    void Start()
    {
        currentHealth = maximumHealth;

        skinnedMeshRenderers = GetComponentsInChildren<MeshRenderer>().ToList();
        if (skinnedMeshRenderers != null)
        {
            foreach(MeshRenderer skinnedMeshRenderer in skinnedMeshRenderers)
            {
                foreach (Material material in skinnedMeshRenderer.materials)
                {
                    skinnedMaterials.Add(material);
                }
            }
        }   
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

    public void TakeDamage(float damage, GameObject source)
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
            damageEvent.Invoke(source);
            

        }
    }

    public void FullHeal()
    {
        currentHealth = maximumHealth;
    }

    public void Die()
    {
        if (dissolveActive)
        {
            StartCoroutine(DeathDissolve());
            deathEvent.Invoke();
        }
    }

    void UpdateDissolve()
    {
        if(dissolveActive)
        {
            timeSinceTargetAdjusted += Time.deltaTime;
            dissolveSmoothFac = Mathf.Min(1f, timeSinceTargetAdjusted / 1f);

            currentDissolveAmount = Mathf.SmoothStep(currentDissolveAmount, targetDissolveAmount, dissolveSmoothFac);
            for (int i = 0; i < skinnedMaterials.Count; i++)
            {
                skinnedMaterials[i].SetFloat("_DissolveAmount", currentDissolveAmount);
            }
        }
    }
        




    // Update is called once per frame


    IEnumerator DeathDissolve()
    {
        if (skinnedMaterials.Count > 0)
        {
            while (targetDissolveAmount < 1)
            {
                targetDissolveAmount += deathDissolveRate;
                yield return new WaitForSeconds(0.02f);
            }
        }

        //Destroy(gameObject);
    }
}
