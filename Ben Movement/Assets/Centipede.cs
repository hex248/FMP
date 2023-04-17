using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Centipede : MonoBehaviour
{

    [Header("AI")] 
    [SerializeField] private GameObject target;

    private Quaternion wigglelessRotation;


    [Header("Movement")] 
    [SerializeField] private float turnSpeed;
    [SerializeField] private float wiggleSpeed;
    [SerializeField] private float wiggleAmplitude;
    [SerializeField] private float moveSpeed;
    
    [Header("Segment Settings")]
    [SerializeField] List<GameObject> centipedeParts;
    [SerializeField] private float maxPartDistance;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < centipedeParts.Count; i++)
        {
            //start from the last one
            int partIndex = centipedeParts.Count - 1 - i;
            if (partIndex == 0)
            {
                //do regular movement
                GameObject part = centipedeParts[partIndex];
                Vector3 offset = target.transform.position - part.transform.position;
                offset = new Vector3(offset.x, 0f, offset.z);
                
                Quaternion targetRotation = Quaternion.LookRotation(offset, Vector3.up);
                wigglelessRotation = Quaternion.RotateTowards(wigglelessRotation, targetRotation, turnSpeed);
                
                float wiggleFac = Mathf.Sin(Time.time * wiggleSpeed);
                float wiggleAmount = wiggleFac * wiggleAmplitude;
                Quaternion wiggleRotation = Quaternion.Euler(0f, wiggleAmount, 0f);
                
                //apply that wiggle
                part.transform.rotation = wigglelessRotation * wiggleRotation;
                
                part.transform.position += part.transform.forward * (Time.deltaTime * moveSpeed);
            }
            else
            {
                //face part in front and move forwards
                GameObject part = centipedeParts[partIndex];
                GameObject followPart = centipedeParts[partIndex - 1];
                Vector3 offset = followPart.transform.position - part.transform.position;

                part.transform.forward = offset.normalized;
                if (offset.magnitude > maxPartDistance)
                {
                    Vector3 move = (offset.magnitude - maxPartDistance) * offset.normalized;
                    part.transform.position = part.transform.position + move;
                }
            }
            
        }
    }
}
