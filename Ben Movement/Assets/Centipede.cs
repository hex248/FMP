using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Centipede : MonoBehaviour
{
    
    
    [Header("Movement")]
    private float currentMoveSpeed;
    private float randomFactor;
    [SerializeField] private float turnSpeed;
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
            Debug.Log("process centipede segment " + partIndex);
            if (partIndex == 0)
            {
                //do regular movement
                float turnFac = Mathf.Sin(Time.time);
                GameObject part = centipedeParts[partIndex];
                float turnAmount = turnFac * Time.deltaTime * turnSpeed;
                
                part.transform.eulerAngles = new Vector3(0f, turnAmount, 0f) + part.transform.eulerAngles;
                part.transform.position += part.transform.right * (Time.deltaTime * moveSpeed);
            }
            else
            {
                //face part in front and move forwards
                GameObject part = centipedeParts[partIndex];
                GameObject followPart = centipedeParts[partIndex - 1];
                Vector3 offset = followPart.transform.position - part.transform.position;

                part.transform.right = offset.normalized;
                if (offset.magnitude > maxPartDistance)
                {
                    Vector3 move = (offset.magnitude - maxPartDistance) * offset.normalized;
                    part.transform.position = part.transform.position + move;
                }
            }
            
        }
    }
}
