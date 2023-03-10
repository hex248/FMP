using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowDisplacementScript : MonoBehaviour
{
    Mesh mesh;
    MeshFilter meshFilter;
    //List<Vector3> vertices = new List<Vector3>();
    //List<Color> colors = new List<Color>();

    SnowManager snowManager;

    bool highPoly = false;
    // Start is called before the first frame update
    void Start()
    {
        snowManager = FindObjectOfType<SnowManager>();
        meshFilter = GetComponent<MeshFilter>();
    }

    // Update is called once per frame
    void Update()
    {
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!highPoly)
            {
                SwitchToHighPoly();
            }
            else
            {
                List<Vector3> vertices = new List<Vector3>();
                meshFilter.mesh.GetVertices(vertices);
                List<Color> colors = new List<Color>();
                meshFilter.mesh.GetColors(colors);

                for (int i = 0; i < vertices.Count - 1; i++)
                {
                    if (Vector3.Distance(vertices[i], other.transform.position) <= snowManager.effectRange)
                    {
                        Debug.Log(vertices[i]);
                        vertices[i] += Vector3.down * Time.deltaTime * 40f;
                        Debug.Log(vertices[i]);
                        /*colors[i] = new Color(meshFilter.mesh.normals[i].x, meshFilter.mesh.normals[i].y, meshFilter.mesh.normals[i].z);
                        */
                        if (vertices[i].y > -2f && vertices[i].y < 2f)
                        {
                            
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
                //meshFilter.mesh.SetColors(colors);
                meshFilter.mesh.RecalculateNormals();
            }
        }
    }

    void SwitchToHighPoly()
    {
        Debug.Log("Switch to high poly");
        meshFilter.mesh = snowManager.highPolyMesh;
        highPoly = true;
    }
}
