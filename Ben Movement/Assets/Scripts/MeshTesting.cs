using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshTesting : MonoBehaviour
{
    public bool enabled = false;
    public GameObject[] toDisable;

    void Start()
    {
        
    }

    void Update()
    {
        for (int i = 0; i < toDisable.Length; i++)
        {
            Renderer[] renderers = toDisable[i].GetComponentsInChildren<Renderer>();
            Collider[] colliders = toDisable[i].GetComponentsInChildren<Collider>();
            for (int j = 0; j < renderers.Length; j++)
            {
                renderers[j].enabled = !enabled;
            }
            for (int j = 0; j < colliders.Length; j++)
            {
                colliders[j].enabled = !enabled;
            }
        }
    }
}
