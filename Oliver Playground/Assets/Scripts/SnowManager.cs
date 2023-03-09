using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowManager : MonoBehaviour
{
    PlayerManager playerManager;

    public GameObject snowPrefab;
    public Mesh lowPolyMesh;
    public Mesh highPolyMesh;

    public float step;

    public int size = 3;

    private void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();

        PlaceSnow();
    }

    void PlaceSnow()
    {
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                MeshFilter meshFilter = Instantiate(snowPrefab, transform.position + new Vector3(step * x, 0, step * y), Quaternion.identity, transform).GetComponent<MeshFilter>();

                meshFilter.mesh = lowPolyMesh;
            }
        }
    }
}
