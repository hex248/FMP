using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowManager : MonoBehaviour
{
    MeshFilter meshFilter;
    public List<Vector3> vertices = new List<Vector3>();
    public List<Vector3> currentMeshVertices = new List<Vector3>();
    List<Color> colors = new List<Color>();
    // Start is called before the first frame update
    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.sharedMesh.GetVertices(vertices);

        DrawCircleFromPoint(Vector3.zero, 2f);
        //mesh.GetColors(colors);
        for (int i = 0; i < vertices.Count - 1; i++)
        {
            //vertices[i] += Vector3.up * Mathf.PerlinNoise(vertices[i].x, vertices[i].z) * 0.25f;
            //mesh.RecalculateNormals();
            //colors[i] = new Color(mesh.normals[i].x, mesh.normals[i].y, mesh.normals[i].z);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //mesh.Clear();

        //mesh.SetVertices(vertices);
        meshFilter.mesh.GetVertices(currentMeshVertices);

        //mesh.SetColors(colors);
        //mesh.RecalculateNormals();
    }
    private void OnTriggerStay(Collider other)
    {
        DrawCircleFromPoint(other.transform.position, 2f);
    }

    void DrawCircleFromPoint(Vector3 center, float radius)
    {
        int vertexCount = 15;

        float x;
        float y;
        float z;

        float angle = 20f;
        Debug.Log(angle);

        Mesh originalMesh = meshFilter.sharedMesh;
        Mesh clonedMesh = new Mesh();

        clonedMesh.name = "clone";
        clonedMesh.triangles = originalMesh.triangles;
        clonedMesh.normals = originalMesh.normals;
        clonedMesh.uv = originalMesh.uv;

        for (int i = 0; i < (vertexCount + 1); i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            z = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

            Vector3 vertexPosition = new Vector3(center.x + x, vertices[0].y + 2, center.z + z);

            vertices.Add(vertexPosition);
            Debug.Log(vertexPosition);

            angle += (360f / vertexCount - 1);
        }

        clonedMesh.vertices = vertices.ToArray();

        meshFilter.mesh = clonedMesh;

        // https://www.kodeco.com/3169311-runtime-mesh-manipulation-with-unity#toc-anchor-001
    }
}
