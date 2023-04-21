using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class TrailDrawer : MonoBehaviour
{
    public string name;
    public Transform drawTransform;
    public float drawSize = 5;

}
