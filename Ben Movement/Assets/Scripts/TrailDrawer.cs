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

    public ObjectType objectType;

    private void OnDestroy()
    {
        // send message to TerrainInteractiveSnow to remove this from the 
        FindObjectOfType<TerrainInteractiveSnow>().RemoveObject(this.gameObject);
    }
}
