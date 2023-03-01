using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decal : MonoBehaviour
{
    public float initialAlpha;
    public float timeUntilFade;
    public float fadeTime;
    float timeElapsed = 0.0f;
    Renderer renderer;

    private void Start()
    {
        renderer = GetComponent<Renderer>();
        renderer.material.SetFloat("_Alpha", initialAlpha);
    }

    private void LateUpdate()
    {
        timeElapsed += Time.deltaTime;
        if (timeUntilFade <= timeElapsed)
        {
            float alpha = initialAlpha - (initialAlpha * ((timeElapsed - timeUntilFade) / fadeTime));
            renderer.material.SetFloat("_Alpha", alpha);
            if (alpha <= 0) Destroy(gameObject); // if decal is invisible, destroy this game object
        }
    }
}
