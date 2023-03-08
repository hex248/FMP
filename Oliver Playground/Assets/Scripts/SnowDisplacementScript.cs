using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowDisplacementScript : MonoBehaviour
{
    Mesh mesh;
    List<Vector3> vertices = new List<Vector3>();
    List<Color> colors = new List<Color>();
    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        mesh.GetVertices(vertices);
        mesh.GetColors(colors);
        for (int i = 0; i < vertices.Count - 1; i++)
        {
            //vertices[i] += Vector3.up * Mathf.PerlinNoise(vertices[i].x, vertices[i].z) * 0.25f;
            mesh.RecalculateNormals();
            colors[i] = new Color(mesh.normals[i].x, mesh.normals[i].y, mesh.normals[i].z);
        }
    }

    // Update is called once per frame
    void Update()
    {
        mesh.SetVertices(vertices);
        mesh.SetColors(colors);
        mesh.RecalculateNormals();
    }
    private void OnTriggerStay(Collider other)
    {
        for (int i = 0; i < vertices.Count - 1; i++)
        {
            if (other.bounds.max.x >= (Vector3.Scale(vertices[i], transform.lossyScale) + transform.position).x && other.bounds.min.x <= (Vector3.Scale(vertices[i], transform.lossyScale) + transform.position).x && other.bounds.max.z >= (Vector3.Scale(vertices[i], transform.lossyScale) + transform.position).z && other.bounds.min.z <= (Vector3.Scale(vertices[i], transform.lossyScale) + transform.position).z)
            {
                if (vertices[i].y > -2f && vertices[i].y < 2f)
                {
                    vertices[i] += Vector3.down * Time.deltaTime * 10f;
                    colors[i] = new Color(mesh.normals[i].x, mesh.normals[i].y, mesh.normals[i].z);
                }
            }
        }
    }
}
