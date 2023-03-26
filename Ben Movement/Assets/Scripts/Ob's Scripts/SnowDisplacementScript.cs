using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowDisplacementScript : MonoBehaviour
{
    Mesh mesh;
    MeshFilter meshFilter;
    List<Vector3> vertices = new List<Vector3>();
    List<Color> colors = new List<Color>();
    List<int> collidedVertsIndexes = new List<int>();

    SnowManager snowManager;

    bool highPoly = false;
    Vector3[] originalVertices;
    Color originalColor;

    public Collider player;
    // Start is called before the first frame update
    void Start()
    {
        snowManager = FindObjectOfType<SnowManager>();
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh.GetVertices(vertices);
        meshFilter.mesh.GetColors(colors);
        originalVertices = snowManager.highPolyMesh.vertices;
        originalColor = colors[0];
    }

    // Update is called once per frame
    void Update()
    {
        if (highPoly)
        {
            if (collidedVertsIndexes.Count > 0)
            {
                for (int i = 0; i < collidedVertsIndexes.Count; i++)
                {
                    int j = collidedVertsIndexes[i];
                    if (vertices[j].y < originalVertices[j].y)
                    {
                        vertices[j] += Vector3.up * Time.deltaTime * 0.5f;
                        colors[j] = new Color(meshFilter.mesh.normals[j].x, meshFilter.mesh.normals[j].y, meshFilter.mesh.normals[j].z);
                    }
                    else
                    {
                        collidedVertsIndexes.Remove(j);
                        colors[j] = originalColor;
                    }
                }
            }
            else
            {
                SwitchToLowPoly();
            }
        }
    }

    private void LateUpdate()
    {
        meshFilter.mesh.SetVertices(vertices);
        meshFilter.mesh.SetColors(colors);
        meshFilter.mesh.RecalculateNormals();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!highPoly)
        {
            SwitchToHighPoly();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        for (int i = 0; i < vertices.Count; i++)
        {
            if (other.tag == "Player")
            {
                player = other;
            }
            if (other.bounds.max.x >= (Vector3.Scale(vertices[i], transform.lossyScale) + transform.position).x
                && other.bounds.min.x <= (Vector3.Scale(vertices[i], transform.lossyScale) + transform.position).x
                && other.bounds.max.z >= (Vector3.Scale(vertices[i], transform.lossyScale) + transform.position).z
                && other.bounds.min.z <= (Vector3.Scale(vertices[i], transform.lossyScale) + transform.position).z)
            {
                if (vertices[i].y > -0.5f && vertices[i].y < 0.5f)
                {
                    vertices[i] += Vector3.down * Time.deltaTime * 10.0f;
                    colors[i] = new Color(meshFilter.mesh.normals[i].x, meshFilter.mesh.normals[i].y, meshFilter.mesh.normals[i].z);
                    if (!collidedVertsIndexes.Contains(i) || collidedVertsIndexes.Count == 0)
                    {
                        collidedVertsIndexes.Add(i);
                        break;
                    }
                }
            }
        }
    }

    void SwitchToHighPoly()
    {
        Debug.Log("Switch to high poly");
        meshFilter.mesh = snowManager.highPolyMesh;
        meshFilter.mesh.GetVertices(vertices);
        meshFilter.mesh.GetColors(colors);
        highPoly = true;
    }
    void SwitchToLowPoly()
    {
        Debug.Log("Switch to low poly");
        meshFilter.mesh = snowManager.lowPolyMesh;
        meshFilter.mesh.GetVertices(vertices);
        meshFilter.mesh.GetColors(colors);
        highPoly = false;
    }
}
