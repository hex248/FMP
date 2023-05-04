using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedMaterial : MonoBehaviour
{
    public Material mat;
    public Texture[] frames;
    public float FPS = 24;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    int i = 0;
    float time = 0.0f;

    // Update is called once per frame
    void Update()
    {
        if(time < 1 / FPS)
        {
            time += Time.deltaTime;
        }
        else
        {
            if (i < frames.Length - 1)
            {
                i++;
            }
            else
            {
                i = 0;
            }
            mat.mainTexture = frames[i];
            time = 0.0f;
        }
    }
}
