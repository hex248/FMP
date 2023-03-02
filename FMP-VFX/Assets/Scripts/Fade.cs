using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Fade : MonoBehaviour
{
    [SerializeField][Range(0,1)] float fadeAmount;
    [SerializeField][Range(1,10)] float fadeSpeed;
    [SerializeField][Range(0.0f, 0.1f)] float edgeSize;
    [SerializeField][ColorUsage(true,true)] Color edgeColor;
    [SerializeField] Texture texture;

    private Renderer renderer;
    private Material material;

    void Update()
    {
        if (Application.isPlaying)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                fadeAmount += Time.deltaTime * fadeSpeed;
                fadeAmount = Mathf.Clamp(fadeAmount, 0.0f, 1.0f);
            }
            else
            {
                fadeAmount -= Time.deltaTime * fadeSpeed;
                fadeAmount = Mathf.Clamp(fadeAmount, 0.0f, 1.0f);
            }
        }

        material = GetComponent<Renderer>().material;
        material.SetFloat("DissolveAmount", fadeAmount);
        material.SetFloat("EdgeSize", edgeSize);
        material.SetColor("EdgeColor", edgeColor);
        material.SetTexture("Texture", texture);
    }
}
