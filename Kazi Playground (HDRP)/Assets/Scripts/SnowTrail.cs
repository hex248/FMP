using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowTrail : MonoBehaviour
{
    float timeMoving;
    Rigidbody rb;
    Transform snowTrailsParent;

    public GameObject decalPrefab;

    public bool fade = true;
    public float timeUntilFade;
    public float fadeTime;

    List<Decal> decalsCreated = new List<Decal>();

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        snowTrailsParent = GameObject.Find("Snow Trails").transform;
    }

    void Update()
    {
        if (rb.velocity.magnitude > 0.01)
        {
            PaintDecal();
        }
    }

    public void PaintDecal()
    {
        GameObject spawnedObject = Instantiate(decalPrefab, transform.position, Quaternion.identity, snowTrailsParent);

        Decal decal = spawnedObject.GetComponent<Decal>();

        decalsCreated.Add(decal);
    }
}
