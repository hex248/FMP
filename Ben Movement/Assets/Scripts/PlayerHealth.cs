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

    [Header("Vignette Setting")]
    public Volume volume;
    Vignette vignette;

    [SerializeField] float hurtGradientAmount;
    [SerializeField] float hurtIntensityAmount;

    float currentTargetGradientAmount;
    float currentTargetIntensityAmount;

    [SerializeField] float vignetteSmoothSpeed;
    [SerializeField] Color vignetteColor;



    private void Start()
    {
        currentHitPoints = maxHitPoints;
        controller = GetComponent<PlayerController>();
        anim = GetComponentInChildren<PlayerAnimationScript>();
        player = transform.parent.GetComponent<Player>();
        cameraLocation = player.cameraFollow;
        volume = cameraLocation.GetComponentInChildren<Volume>();
        vignette = volume.profile.Add<Vignette>();
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
        if (invincible)
        {
            currentTargetIntensityAmount = hurtIntensityAmount;
            currentTargetGradientAmount = hurtGradientAmount;
        }
    
        else
        {
            currentTargetIntensityAmount = 0f;
            currentTargetGradientAmount = 0f;
        }
    

        float currentIntensity = (float)vignette.intensity;
        float currentGradient = (float)vignette.smoothness;
        vignette.intensity.Override(Mathf.MoveTowards(currentIntensity, currentTargetIntensityAmount, Time.deltaTime * vignetteSmoothSpeed * (hurtIntensityAmount / 1f)));
        vignette.smoothness.Override(Mathf.MoveTowards(currentGradient, currentTargetGradientAmount, Time.deltaTime * vignetteSmoothSpeed * (hurtGradientAmount / 1f)));
        vignette.color.Override(vignetteColor);
    }

    public void Damage(int amount = 1)
    {
        if (invincible)
            return;

        currentHitPoints -= amount;

        currentHitPoints = Mathf.Clamp(currentHitPoints, 0, maxHitPoints);

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
}
