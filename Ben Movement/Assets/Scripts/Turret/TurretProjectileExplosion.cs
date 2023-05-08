using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretProjectileExplosion : MonoBehaviour
{
    float lifeTime = 0.0f;

    private void Update()
    {
        lifeTime += Time.deltaTime;
        if (lifeTime>=2.0f)
        {
            Destroy(this.gameObject);
        }
    }
}
