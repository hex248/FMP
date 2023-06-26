using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingTrap : MonoBehaviour
{
    AudioManager AM;

    [Header("Visuals")]
    public Material heal;
    public GameObject[] lights;
    public GameObject visuals;
    [SerializeField] GameObject healPack;
    [SerializeField] float cooldown;
    float timeSinceHealingUsed;
    float targetScale;

    Vector3 towerPos;
    bool hasHealing;
    void Start()
    {
        AM = FindObjectOfType<AudioManager>();
        towerPos = visuals.transform.position;
        StartCoroutine(SpawnTurret());
        targetScale = 2f;
    }

    private void OnEnable()
    {
        timeSinceHealingUsed = cooldown;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasHealing)
            return;
        if (other.CompareTag("Player Trigger"))
        {
            // play healing sound in SFX Channel 1
            if (AM.healingSoundOn)
            {
                AM.PlayInChannel("healing-trap_heal", ChannelType.SFX, 1);
            }
            other.GetComponent<PlayerTrigger>().health.Heal(1);
            hasHealing = false;
            targetScale = 0f;

        }
    }

    void Update()
    {
        healPack.transform.localScale = Vector3.MoveTowards(healPack.transform.localScale, targetScale * Vector3.one, Time.deltaTime * 10f);
        if (!hasHealing)
        {
            timeSinceHealingUsed += Time.deltaTime;
            if(timeSinceHealingUsed >= cooldown)
            {
                RespawnHealing();
                
            }
            
        }
        else
        {
            timeSinceHealingUsed = 0f;
        }
        heal.SetColor("_EmissionColor", Color.Lerp(heal.GetColor("_EmissionColor"), new Color(0.0f, 191.0f / 255.0f, 11.0f / 255.0f) * ((timeSinceHealingUsed / cooldown) + (targetScale / 2.0f)) * Mathf.Pow(2.0f, 2.7f), Time.deltaTime * 10f));
    }

    void RespawnHealing()
    {
        hasHealing = true;
        targetScale = 2f;
    }

    IEnumerator SpawnTurret()
    {
        visuals.transform.position = new Vector3(towerPos.x, -10.0f, towerPos.z);
        foreach (GameObject light in lights)
        {
            light.SetActive(false);
        }

        float moveSpeed = 5.0f;
        float distanceToMove = (towerPos - visuals.transform.position).sqrMagnitude;

        for (; ; )
        {
            // move towards turretPos

            visuals.transform.position = Vector3.Lerp(visuals.transform.position, towerPos, moveSpeed * Time.deltaTime);

            if (visuals.transform.position == towerPos)
            {
                break;
            }

            yield return new WaitForEndOfFrame();
        }

        foreach (GameObject light in lights)
        {
            light.SetActive(true);
        }
    }
}
