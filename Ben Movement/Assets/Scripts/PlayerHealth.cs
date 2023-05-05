using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] int maxHitPoints = 5;
    int currentHitPoints;
    [SerializeField] float invincibilityTime;
    float timeSinceHit;
    bool invincible;

    PlayerController controller;
    PlayerAnimationScript anim;
    Player player;

    CameraFollow cameraLocation;
    public Volume volume;
    
    Vignette vignette;

    
    [Header("Hurt Settings")]
    [SerializeField] float hurtVignetteGradient;
    [SerializeField] float hurtVignetteIntensity;
    [SerializeField] Color hurtVignetteColor;
    [SerializeField] float hurtVignetteSmoothSpeed;

    [Header("Death Settings")]
    [SerializeField] GameObject grave;

    [SerializeField] GameObject playerVisuals;
    [SerializeField] float deathVignetteIntensity;
    [SerializeField] float deathVignetteGradient;
    [SerializeField] float deathVignetteSmoothSpeed;
    [SerializeField] Color deathVignetteColor;

    bool dead;
    Color vignetteColor;
    float vignetteSmoothSpeed;
    float currentTargetVignetteGradient;
    float currentTargetVignetteIntensity;
 



    private void Start()
    {
        currentHitPoints = maxHitPoints;
        controller = GetComponent<PlayerController>();
        anim = GetComponentInChildren<PlayerAnimationScript>();
        player = transform.parent.GetComponent<Player>();
        cameraLocation = player.cameraFollow;
        volume = cameraLocation.GetComponentInChildren<Volume>();
        vignette = volume.profile.Add<Vignette>();
        dead = false;
    }

    private void Update()
    {
        if (invincible)
        {
            timeSinceHit += Time.deltaTime;
            if (timeSinceHit >= invincibilityTime)
            {
                invincible = false;
            }

            
        }

        UpdateVignette();

        if(player.playerHealthBar != null)
        {
            player.playerHealthBar.Health = currentHitPoints;
        }
    }

    public void UpdateVignette()
    {
        if(dead)
        {
            currentTargetVignetteIntensity = deathVignetteIntensity;
            currentTargetVignetteGradient = deathVignetteGradient;
            vignetteColor = deathVignetteColor;
            vignetteSmoothSpeed = deathVignetteSmoothSpeed;
        }
        else if (invincible)
        {
            currentTargetVignetteIntensity = hurtVignetteIntensity;
            currentTargetVignetteGradient = hurtVignetteGradient;
            vignetteColor = hurtVignetteColor;
            vignetteSmoothSpeed = hurtVignetteSmoothSpeed;
        }
    
        else
        {
            currentTargetVignetteIntensity = 0f;
            currentTargetVignetteGradient = 0f;
        }
    

        float currentIntensity = (float)vignette.intensity;
        float currentGradient = (float)vignette.smoothness;
        vignette.intensity.Override(Mathf.MoveTowards(currentIntensity, currentTargetVignetteIntensity, Time.deltaTime * vignetteSmoothSpeed * (hurtVignetteIntensity / 1f)));
        vignette.smoothness.Override(Mathf.MoveTowards(currentGradient, currentTargetVignetteGradient, Time.deltaTime * vignetteSmoothSpeed * (hurtVignetteGradient / 1f)));
        vignette.color.Override(vignetteColor);
    }

    public void Damage(int amount = 1)
    {
        if (invincible || dead)
            return;

        currentHitPoints -= amount;

        currentHitPoints = Mathf.Clamp(currentHitPoints, 0, maxHitPoints);
        if(currentHitPoints == 0)
        {
            Die();
        }

        timeSinceHit = 0f;
        invincible = true;
        anim.Damage();
        controller.Damage();
        cameraLocation.CameraShake(0.3f, 0.2f);

        player.playerHealthBar.Health = currentHitPoints;
    }

    void Die()
    {
        //death logic
        player.playerMovement.Die();
        grave.SetActive(true);
        grave.transform.position = transform.position;
        playerVisuals.SetActive(false);
        FindObjectOfType<PlayerManager>().PlayerDied();
        dead = true;

        StartCoroutine(FadeOutShake());
    }

    public void Revive()
    {
        player.playerMovement.Revive();
        grave.SetActive(false);
        //grave.transform.position = transform.position;
        playerVisuals.SetActive(true);
        dead = false;
        Heal(maxHitPoints);
        StartCoroutine(FadeOutShake());
    }

    IEnumerator FadeOutShake()
    {
        cameraLocation.CameraShake(0.2f, 2f);
        yield return new WaitForSeconds(2f);
        cameraLocation.CameraShake(0.1f, 1f);
        yield return new WaitForSeconds(1f);
        cameraLocation.CameraShake(0.05f, 0.5f);
        yield return new WaitForSeconds(0.5f);
        cameraLocation.CameraShake(0.05f, 0.1f);
    }

    

    public void Heal(int amount = 1, bool overHeal = false)
    {
        currentHitPoints += amount;

        if (overHeal)
        {
            currentHitPoints = Mathf.Max(0, currentHitPoints);
        }
        else
        {
            currentHitPoints = Mathf.Clamp(currentHitPoints, 0, maxHitPoints);
        }

        player.playerHealthBar.Health = currentHitPoints;
    }

    public int GetCurrentHitPoints()
    {
        return currentHitPoints;
    }

    public bool IsDead()
    {
        return dead;
    }
}
