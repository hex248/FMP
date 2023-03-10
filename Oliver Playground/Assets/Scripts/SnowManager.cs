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
    public float effectRange = 0.1f;

    public Vector3 startPosition;

    private void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();

        startPosition = transform.position - new Vector3(((size-1) * step) / 2, 0, ((size - 1) * step) / 2);

        PlaceSnow();
    }

    void PlaceSnow()
    {
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                MeshFilter meshFilter = Instantiate(snowPrefab, startPosition + new Vector3(step * x, 0, step * y), Quaternion.identity, transform).GetComponent<MeshFilter>();

                meshFilter.mesh = lowPolyMesh;
            }
        }
    }
}
