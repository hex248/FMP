using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public struct SnowData
{
    public Vector3 position;
    public Color vertexColor;
    public int collided;
}

public class SnowDisplacementCompute : MonoBehaviour
{
    public ComputeShader computeShader;

    SnowData[] snowData;
    public float topOutHeight;
    public float bottomOutHeight = -1.0f;

    Mesh mesh;
    MeshFilter meshFilter;
    public List<Vector3> vertices = new List<Vector3>();
    List<Color> colors = new List<Color>();
    List<int> collidedVertsIndexes = new List<int>();

    SnowManager snowManager;

    public bool highPoly = false;

    Vector3[] originalVertices;
    Color originalColor;

    Collider player;

    private void Start()
    {
        snowManager = FindObjectOfType<SnowManager>();
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh.GetVertices(vertices);
        meshFilter.mesh.GetColors(colors);
        //originalVertices = snowManager.highPolyMesh.vertices;
        //List<Vector3> tempOrigVerts = snowManager.highPolyMesh.vertices.ToList();
        //tempOrigVerts.RemoveRange(tempOrigVerts.Count - 7, tempOrigVerts.Count - 1);
        //originalVertices = tempOrigVerts.ToArray();
        originalColor = colors[0];

        topOutHeight = vertices[0].y;

        snowData = new SnowData[snowManager.highPolyMesh.vertices.Length - 6];

        SwitchToLowPoly();
    }

    void Update()
    {
        if (highPoly)
        {
            if (collidedVertsIndexes.Count > 0)
            {
                // let compute shader deal with this

                /*
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
                */
            }
            else if (1 > 2)
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
        // compute shader takes care of next steps
        if (highPoly)
        {
            ComputeVerticesGPU(other);
        }
        else
        {
            SwitchToHighPoly();
        }


        /*
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
        */
    }

    public void ComputeVerticesGPU(Collider other)
    {
        int positionSize = sizeof(float) * 3;
        int colorSize = sizeof(float) * 4;
        int collidedSize = sizeof(int) * 1;
        int totalSize = (positionSize + colorSize + collidedSize)/* * snowData.Length*/;
        //Debug.Log(totalSize);

        ComputeBuffer snowDataBuffer = new ComputeBuffer(snowData.Length, totalSize);
        snowDataBuffer.SetData(snowData);
        computeShader.SetBuffer(0, "snowData", snowDataBuffer);
        computeShader.SetFloat("resolution", snowData.Length);
        computeShader.SetFloat("bottomOutHeight", bottomOutHeight);
        computeShader.SetFloat("topOutHeight", topOutHeight);
        computeShader.SetVector("collidingBoundsMax", other.bounds.max);
        computeShader.SetVector("collidingBoundsMin", other.bounds.min);

        //computeShader.SetVector("_Time", Shader.GetGlobalVector("_Time"));

        computeShader.Dispatch(0, snowData.Length / 10, 1, 1);

        snowDataBuffer.GetData(snowData);

        collidedVertsIndexes = new List<int>();
        for (int i = 0; i < snowData.Length; i++)
        {
            vertices[i] = snowData[i].position;
            colors[i] = snowData[i].vertexColor;
            if (snowData[i].collided > 0)
            {
                collidedVertsIndexes.Add(i);
            }
        }

        Debug.Log(vertices.Count);
        Debug.Log(colors.Count);
        Debug.Log(snowData.Length);

        snowDataBuffer.Dispose();

    }

    void SwitchToHighPoly()
    {
        Debug.Log("switch to high poly");
        meshFilter.mesh = snowManager.highPolyMesh;
        meshFilter.mesh.GetVertices(vertices);
        vertices.RemoveRange(vertices.Count - 7, vertices.Count - 1);
        meshFilter.mesh.GetColors(colors);
        colors.RemoveRange(vertices.Count - 7, vertices.Count - 1);

        for (int i = 0; i < vertices.Count; i++)
        {
            SnowData data = new SnowData();
            data.position = vertices[i];
            data.vertexColor = colors[i];
            data.collided = -1;
            snowData[i] = data;
        }

        highPoly = true;
    }
    void SwitchToLowPoly()
    {
        Debug.Log("switch to low poly");
        meshFilter.mesh = snowManager.lowPolyMesh;
        meshFilter.mesh.GetVertices(vertices);
        meshFilter.mesh.GetColors(colors);

        for (int i = 0; i < vertices.Count; i++)
        {
            SnowData data = new SnowData();
            data.position = vertices[i];
            data.vertexColor = colors[i];
            data.collided = -1;
            snowData[i] = data;
        }

        highPoly = false;
    }
}
