using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectType
{
    Player,
    Environment,
    Enemy
}

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class TrailDrawer : MonoBehaviour
{
    public string name;
    public Transform drawTransform;
    public float drawSize = 5;
    public Texture2D drawBrush;
    public float drawDistance = 1f;
    public int index = -1;
    public bool isEnabled = true;

    public ObjectType objectType;

    private void OnDestroy()
    {
        // send message to TerrainInteractiveSnow to remove this from the 
        if (FindObjectOfType<TerrainInteractiveSnow>())
            FindObjectOfType<TerrainInteractiveSnow>().RemoveObject(index);
    }

    private void OnDisable()
    {
        if (FindObjectOfType<TerrainInteractiveSnow>())
            FindObjectOfType<TerrainInteractiveSnow>().RemoveObject(index);
    }
}
