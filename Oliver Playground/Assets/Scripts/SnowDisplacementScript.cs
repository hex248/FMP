using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowDisplacementScript : MonoBehaviour
{
    Mesh mesh;
    Vector3[] vertices;
    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;
    }

    // Update is called once per frame
    void Update()
    {
        mesh.vertices = vertices;
    }
    private void OnTriggerStay(Collider other)
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            if (other.bounds.max.x >= (Vector3.Scale(vertices[i], transform.lossyScale) + transform.position).x && other.bounds.min.x <= (Vector3.Scale(vertices[i], transform.lossyScale) + transform.position).x && other.bounds.max.z >= (Vector3.Scale(vertices[i], transform.lossyScale) + transform.position).z && other.bounds.min.z <= (Vector3.Scale(vertices[i], transform.lossyScale) + transform.position).z)
            {
                if (vertices[i].y > -2f && vertices[i].y < 2f)
                {
                    vertices[i] += Vector3.down * Time.deltaTime * 10f;
                    mesh.RecalculateNormals();
                }
            }
        }
    }
}
