using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] float maximumHealth;
    [SerializeField] float currentHealth;
    float healthFactor;

    List<SkinnedMeshRenderer> skinnedMeshRenderers;
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
    bool isDead;

    [SerializeField] UnityEvent<GameObject> damageEvent;
    [SerializeField] UnityEvent deathEvent;

    WaveManager enemySpawner;

    public int essenceDropAmount;
    public int essenceParticleCount = 1;
    [SerializeField] GameObject essencePrefab;

    void Start()
    {
        currentHealth = maximumHealth;
        isDead = false;
        skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>().ToList();
        if (skinnedMeshRenderers != null)
            foreach(SkinnedMeshRenderer skinnedMeshRenderer in skinnedMeshRenderers)
            {
                foreach (Material material in skinnedMeshRenderer.materials)
                {
                    skinnedMaterials.Add(material);
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

        if (currentHealth == 0f && !isDead)
        {
            Die();
            isDead = true;
        }
        else if(!isDead)
        {
            healthFactor = 1 - currentHealth / maximumHealth;

            targetDissolveAmount = healthFactor * maxDamageDissolveAmount;
            dissolveDistance = Mathf.Abs(currentDissolveAmount - targetDissolveAmount);

            timeSinceTargetAdjusted = 0f;
            damageEvent.Invoke(source);
            

        }
    }

    public void Die()
    {
        if (dissolveActive)
        {
            StartCoroutine(DeathDissolve());
            SpawnEssence();
            deathEvent.Invoke();
            enemySpawner.EnemyDeath();
        }
    }

    public void SpawnEssence()
    {
        for (int i = 0; i < essenceParticleCount; i++)
        {
            GameObject spawnedObj = Instantiate(essencePrefab, transform.position, Quaternion.identity, GameObject.Find("All Essence Drops").transform);
            spawnedObj.GetComponent<DreamEssencePickup>().essenceAmount = essenceDropAmount;
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

        Destroy(gameObject);
    }

    public void LinkSpawner(WaveManager spawner)
    {
        enemySpawner = spawner;
    }
}
