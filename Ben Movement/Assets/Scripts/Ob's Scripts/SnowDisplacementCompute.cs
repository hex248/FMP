using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public struct SnowData
{
    public Vector3 position;
    public Color vertexColor;
    public float moveAmount;
}

public class SnowDisplacementCompute : MonoBehaviour
{
    public ComputeShader computeShader;

    SnowData[] snowData;
    public float topOutHeight;
    public float bottomOutHeight = -1.0f;

    public float downSpeed = -5f;
    public float upSpeed = 2f;

    Mesh mesh;
    MeshFilter meshFilter;
    public List<Vector3> vertices = new List<Vector3>();
    List<Color> colors = new List<Color>();
    public List<int> collidedVertsIndexes = new List<int>();

    SnowManager snowManager;

    public bool highPoly = false;
    public bool colliding;

    private void Start()
    {
        snowManager = FindObjectOfType<SnowManager>();
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh.GetVertices(vertices);
        meshFilter.mesh.GetColors(colors);

        topOutHeight = vertices[0].y;

        snowData = new SnowData[snowManager.highPolyMesh.vertices.Length];
    }

    void Update()
    {
        colliding = false;
    }

    private void LateUpdate()
    {
        meshFilter.mesh.SetVertices(vertices);
        meshFilter.mesh.SetColors(colors);
        meshFilter.mesh.RecalculateNormals();

        if (highPoly)
        {
            // compute rising vertices
            if (!colliding)
            {
                ComputeVerticesGPU();
            }
            // if all vertices have realigned with the top out height
            if (collidedVertsIndexes.Count < 1 && !colliding)
            {
                SwitchToLowPoly();
            }
        }
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
        colliding = true;
        ComputeVerticesGPU(other);
    }
    private void OnTriggerExit(Collider other)
    {
        colliding = false;
    }

    public void ComputeVerticesGPU()
    {
        for (int i = 0; i < collidedVertsIndexes.Count; i++)
        {
            snowData[collidedVertsIndexes[i]].moveAmount = upSpeed;
        }

        int positionSize = sizeof(float) * 3;
        int colorSize = sizeof(float) * 4;
        int moveAmountSize = sizeof(float) * 1;
        int totalSize = (positionSize + colorSize + moveAmountSize);
        

        ComputeBuffer snowDataBuffer = new ComputeBuffer(snowData.Length, totalSize);
        snowDataBuffer.SetData(snowData);
        computeShader.SetBuffer(0, "snowData", snowDataBuffer);
        computeShader.SetFloat("resolution", snowData.Length);
        computeShader.SetFloat("bottomOutHeight", bottomOutHeight);
        computeShader.SetFloat("topOutHeight", topOutHeight);
        computeShader.SetFloat("deltaTime", Time.deltaTime);

        computeShader.Dispatch(0, snowData.Length / 64, 1, 1);

        snowDataBuffer.GetData(snowData);

        for (int i = 0; i < snowData.Length; i++)
        {
            vertices[i] = snowData[i].position;
            colors[i] = snowData[i].vertexColor;
            if (vertices[i].y == topOutHeight)
            {
                collidedVertsIndexes.Remove(i);
            }
        }

        snowDataBuffer.Dispose();
        
    }
    public void ComputeVerticesGPU(Collider other)
    {
        for (int i = 0; i < snowData.Length; i++)
        {
            Vector3 collidingBoundsMax = other.bounds.max;
            Vector3 collidingBoundsMin = other.bounds.min;
            Vector3 pos = transform.position;
            Vector3 vertex = vertices[i];

            if (collidingBoundsMax.x >= (vertex + pos).x
                && collidingBoundsMin.x <= (vertex + pos).x
                && collidingBoundsMax.z >= (vertex + pos).z
                && collidingBoundsMin.z <= (vertex + pos).z)
            {
                snowData[i].moveAmount = downSpeed;
                if (!collidedVertsIndexes.Contains(i))
                {
                    collidedVertsIndexes.Add(i);
                }
            }
            else
            {
                snowData[i].moveAmount = upSpeed;
            }
        }

        int positionSize = sizeof(float) * 3;
        int colorSize = sizeof(float) * 4;
        int moveAmountSize = sizeof(float) * 1;
        int totalSize = (positionSize + colorSize + moveAmountSize);
        

        ComputeBuffer snowDataBuffer = new ComputeBuffer(snowData.Length, totalSize);
        snowDataBuffer.SetData(snowData);
        computeShader.SetBuffer(0, "snowData", snowDataBuffer);
        computeShader.SetFloat("resolution", snowData.Length);
        computeShader.SetFloat("bottomOutHeight", bottomOutHeight);
        computeShader.SetFloat("topOutHeight", topOutHeight);
        computeShader.SetFloat("deltaTime", Time.deltaTime);

        computeShader.Dispatch(0, snowData.Length / 64, 1, 1);

        snowDataBuffer.GetData(snowData);

        for (int i = 0; i < snowData.Length; i++)
        {
            vertices[i] = snowData[i].position;
            colors[i] = snowData[i].vertexColor;
        }

        snowDataBuffer.Dispose();
        
    }

    void SwitchToHighPoly()
    {
        meshFilter.mesh = snowManager.highPolyMesh;
        meshFilter.mesh.GetVertices(vertices);
        meshFilter.mesh.GetColors(colors);

        for (int i = 0; i < vertices.Count; i++)
        {
            SnowData data = new SnowData();
            data.position = vertices[i];
            data.vertexColor = colors[i];
            snowData[i] = data;
        }

        highPoly = true;
    }

    void SwitchToLowPoly()
    {
        meshFilter.mesh = snowManager.lowPolyMesh;
        meshFilter.mesh.GetVertices(vertices);
        meshFilter.mesh.GetColors(colors);

        for (int i = 0; i < vertices.Count; i++)
        {
            SnowData data = new SnowData();
            data.position = vertices[i];
            data.vertexColor = colors[i];
            //data.collided = -1;
            snowData[i] = data;
        }

        highPoly = false;
    }
}
