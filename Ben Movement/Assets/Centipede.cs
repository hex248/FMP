using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Centipede : MonoBehaviour
{
    [SerializeField] List<GameObject> centipedeParts;
    [SerializeField] private float maxPartDistance;
    private float currentMoveSpeed;
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
