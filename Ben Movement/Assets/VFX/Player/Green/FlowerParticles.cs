using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class FlowerParticles : MonoBehaviour
{
    VisualEffect VFX;

    [Header("Settings")]
    [SerializeField] Vector3 spawnOffset;
    [SerializeField] Texture2D[] flowers;
    public Texture texture;
    public int index = 0;

    private void Start()
    {
        VFX = GetComponent<VisualEffect>();
    }
    
    void Update()
    {
        VFX.SetVector3("spawnPosition", transform.position + spawnOffset);
        VFX.SetVector3("direction", transform.right);
        texture = flowers[index];
        index++;
        if (index >= flowers.Length) index = 0;
    }
}
