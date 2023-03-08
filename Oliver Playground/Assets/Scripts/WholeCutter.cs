using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WholeCutter : MonoBehaviour
{
    MeshFilter meshFilter;
    Mesh clonedMesh;

    [Header("Settings")]
    public float radius = 5.0f;
    public float vertexCount = 15.0f;

    [Header("Data")]
    public bool drawnCircle = false;
    public List<Vector3> vertices;
    public List<Vector3> newVertices;
    public List<int> triangles;
    public Vector3[] currentVertices;
    public int[] currentTriangles;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        clonedMesh = new Mesh();

        Mesh originalMesh = meshFilter.mesh;
        clonedMesh.name = $"(CLONE) {originalMesh.name}";
        clonedMesh.vertices = originalMesh.vertices;
        vertices = originalMesh.vertices.ToList();
        clonedMesh.triangles = originalMesh.triangles;
        triangles = originalMesh.triangles.ToList();
        clonedMesh.normals = originalMesh.normals;
        clonedMesh.uv = originalMesh.uv;

        meshFilter.mesh = clonedMesh;
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        currentVertices = clonedMesh.vertices;
        currentTriangles = clonedMesh.triangles;

        if (!drawnCircle)
        {
            //CutCircle(transform.position, radius);
            float size = 5.0f;
            CutSquare(transform.position, size);
        }
    }

    void CutSquare(Vector3 center, float size)
    {
        float x;
        float y;
        float z;

        float angle = 20f;

        for (int i = 0; i < (15 + 1); i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            z = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

            Vector3 vertexPosition = new Vector3(center.x + x, 3, center.z + z);

            vertices.Add(vertexPosition);
            newVertices.Add(vertexPosition);

            angle += (360f / 15);
        }

        for (int i = 0; i < newVertices.Count; i++)
        {
            int[] newTriangle = GetTriangle(newVertices[i]);
            //newTriangle = ClockwiseTriangle(newTriangle);
            for (int j = 0; j < newTriangle.Length; j++)
            {
                triangles.Add(newTriangle[j]);
            }
        }


        clonedMesh.vertices = vertices.ToArray();
        clonedMesh.triangles = triangles.ToArray();

        clonedMesh.RecalculateNormals();

        drawnCircle = true;
    }

    void CutCircle(Vector3 center, float radius)
    { 

        float x;
        float y;
        float z;

        float angle = 20f;

        for (int i = 0; i < (vertexCount + 1); i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            z = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

            Vector3 vertexPosition = new Vector3(center.x + x, -2, center.z + z);

            vertices.Add(vertexPosition);
            newVertices.Add(vertexPosition);

            angle += (360f / vertexCount);
        }

        // loop through vertices yet to have triangles attached
        for (int i = 0; i < newVertices.Count; i++)
        {
            int[] newTriangle = GetTriangle(newVertices[i]);
            //newTriangle = ClockwiseTriangle(newTriangle);
            for (int j = 0; j < newTriangle.Length; j++)
            {
                triangles.Add(newTriangle[j]);
            }
        }

        clonedMesh.vertices = vertices.ToArray();
        clonedMesh.triangles = triangles.ToArray();

        clonedMesh.RecalculateNormals();

        drawnCircle = true;
    }

    int[] GetTriangle(Vector3 vertexPosition)
    {
        Vector4[] closestVertices = new Vector4[]
        { 
            new Vector4(0,0,0,100000),
            new Vector4(0,0,0,100000),
        };

        /*
         * x
         * y
         * z
         * distance
         */

        Vector3[] verticesAvailable = currentVertices;

        Debug.Log(verticesAvailable.Length);
        for (int i = 0; i < verticesAvailable.Length; i++)
        {
            Debug.Log("A");
            float distance = Vector3.Distance(verticesAvailable[i], vertexPosition);
            Debug.Log($"2. Distance: {distance} of Point ({verticesAvailable[i].x},{verticesAvailable[i].z}) to Point ({vertexPosition.x},{vertexPosition.z})");

            if (distance < closestVertices[1].w)
            {
                if (distance < closestVertices[0].w)
                {
                    closestVertices[1] = closestVertices[0];
                    closestVertices[0] = new Vector4(verticesAvailable[i].x, verticesAvailable[i].y, verticesAvailable[i].z, distance);
                }
                else
                {
                    closestVertices[1] = new Vector4(verticesAvailable[i].x, verticesAvailable[i].y, verticesAvailable[i].z, distance);
                }
            }
        }


        int firstIDX = vertices.IndexOf(vertexPosition);
        Debug.Log($"1. Index of ({vertexPosition.x}, {vertexPosition.z}) is {firstIDX}");
        int secondIDX = vertices.IndexOf(closestVertices[0]);
        Debug.Log($"2. Index of ({closestVertices[0].x}, {closestVertices[0].z}) is {secondIDX}");
        int thirdIDX = vertices.IndexOf(closestVertices[1]);
        Debug.Log($"3. Index of ({closestVertices[1].x}, {closestVertices[1].z}) is {thirdIDX}");

        return new int[] { firstIDX, secondIDX, thirdIDX };
        
        
        /*
        Vector3[] closestVertices = new Vector3[2];

        Vector3[] verticesAvailable = currentVertices;

        //verticesAvailable = vertices.ToArray();

        for (int i = 0; i < verticesAvailable.Length; i++)
        {
            if (verticesAvailable[i] != vertexPosition)
            {
                float distance = Vector3.Distance(verticesAvailable[i], vertexPosition);
                // if selected vertex is closer than the further current closest vertex
                if (distance < Vector3.Distance(closestVertices[1], vertexPosition))
                {
                    // if selected vertex is closer than the closer current closest vertex
                    if (distance < Vector3.Distance(closestVertices[0], vertexPosition))
                    {
                        closestVertices[1] = closestVertices[0]; // move down
                        closestVertices[0] = verticesAvailable[i]; // replace
                    }
                    else
                    {
                        closestVertices[1] = verticesAvailable[i];
                    }
                }
            }
        }

        //Debug.Log($"Closest vertex to ({vertexPosition.x}, {vertexPosition.y}, {vertexPosition.z}) is ({vertices[closestVertices[0]].x}, {vertices[closestVertices[0]].y}, {vertices[closestVertices[0]].z})");

        return new int[] { vertices.IndexOf(closestVertices[0]), vertices.IndexOf(vertexPosition), vertices.IndexOf(closestVertices[1]) };
        */


    }

    int[] ClockwiseTriangle(int[] triangle)
    {
        int[] clockwise = new int[3];

        Vector3 triangleMidpoint = (vertices[triangle[0]] + vertices[triangle[1]] + vertices[triangle[2]]) / 3;

        List<Vector4> pointsWithAngle = new List<Vector4>();

        for (int i = 0; i < triangle.Length; i++)
        {
            // calculate angle from midpoint
            float angle = Vector3.Angle(transform.forward, vertices[triangle[i]] - triangleMidpoint);
            //float angle = Mathf.Atan2(vertices[triangle[i]].y - triangleMidpoint.y, vertices[triangle[i]].x - triangleMidpoint.x) * Mathf.Rad2Deg;

            pointsWithAngle.Add(new Vector4(vertices[triangle[i]].x, vertices[triangle[i]].y, vertices[triangle[i]].z, angle));       
        }

        // sort by angle
        pointsWithAngle = pointsWithAngle.OrderBy(e => -e.w).ToList();

        for (int i = 0; i < clockwise.Length; i++)
        {
            clockwise[i] = vertices.IndexOf(new Vector3(pointsWithAngle[i].x, pointsWithAngle[i].y, pointsWithAngle[i].z));
            Debug.Log($"Triangle: {clockwise[i]} ({pointsWithAngle[i].x}, {pointsWithAngle[i].y}, {pointsWithAngle[i].z}) ({pointsWithAngle[i].w}DEG)");
        }
        

        return clockwise;
    }
}
