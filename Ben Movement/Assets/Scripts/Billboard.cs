using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    [SerializeField] Camera alignCamera;

    // Update is called once per frame
    void Update()
    {
        transform.forward = alignCamera.transform.forward;
    } 
}
