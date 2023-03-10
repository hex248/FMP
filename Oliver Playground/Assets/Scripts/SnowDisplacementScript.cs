using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowDisplacementScript : MonoBehaviour
{
    Mesh mesh;
    MeshFilter meshFilter;
    List<Vector3> vertices = new List<Vector3>();
    List<Color> colors = new List<Color>();

    SnowManager snowManager;

    bool highPoly = false;
    // Start is called before the first frame update
    void Start()
    {
        snowManager = FindObjectOfType<SnowManager>();
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh.GetVertices(vertices);
        meshFilter.mesh.GetColors(colors);
    }

    int index = 0;

    // Update is called once per frame
    void Update()
    {
        if(index < vertices.Count - 1)
        {
            index++;
        }
        else
        {
            index = 0;
        }
        if (vertices[index].y > -0.5f && vertices[index].y < 0.5f)
        {
            vertices[index] += Vector3.up * Time.deltaTime * 20f * Mathf.PerlinNoise(vertices[index].x, vertices[index].z);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (!highPoly)
        {
            SwitchToHighPoly();
        }
        else
        {
            for (int i = 0; i < vertices.Count - 1; i++)
            {
                if (other.bounds.max.x >= (Vector3.Scale(vertices[i], transform.lossyScale) + transform.position).x && other.bounds.min.x <= (Vector3.Scale(vertices[i], transform.lossyScale) + transform.position).x && other.bounds.max.z >= (Vector3.Scale(vertices[i], transform.lossyScale) + transform.position).z && other.bounds.min.z <= (Vector3.Scale(vertices[i], transform.lossyScale) + transform.position).z)
                {
                    if (vertices[i].y > -0.5f && vertices[i].y < 0.5f)
                    {
                        colors[i] = new Color(meshFilter.mesh.normals[i].x, meshFilter.mesh.normals[i].y, meshFilter.mesh.normals[i].z);
                        vertices[i] += Vector3.down * Time.deltaTime * 40f;
                    }
                }
                /*if (other.bounds.max.x >= (Vector3.Scale(vertices[i], transform.lossyScale) + transform.position).x && other.bounds.min.x <= (Vector3.Scale(vertices[i], transform.lossyScale) + transform.position).x && other.bounds.max.z >= (Vector3.Scale(vertices[i], transform.lossyScale) + transform.position).z && other.bounds.min.z <= (Vector3.Scale(vertices[i], transform.lossyScale) + transform.position).z)
                {
                    if (vertices[i].y > -2f && vertices[i].y < 2f)
                    {
                        vertices[i] += Vector3.down * Time.deltaTime * 10f;
                        colors[i] = new Color(meshFilter.mesh.normals[i].x, meshFilter.mesh.normals[i].y, meshFilter.mesh.normals[i].z);
                        highPoly = true;
                    }
                }*/
            }

            meshFilter.mesh.SetVertices(vertices);
            meshFilter.mesh.SetColors(colors);
            meshFilter.mesh.RecalculateNormals();
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
}
