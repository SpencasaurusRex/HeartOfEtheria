using System;
using System.Collections.Generic;
using System.Numerics;
using Unity.Profiling;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class ShapeGenerator : MonoBehaviour
{

    MeshFilter meshFilter;
    
    public float Scale = 1;
    public float Stretch = 1;
    public float XFactor = 1;
    public float YFactor = 1;
    public float ZFactor = 1;


    void Update()
    {
        
    }

    void OnValidate()
    {
        Generate();
    }

    void Generate()
    {
        meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        
        Vector3[] baseVertices = new Vector3[8];
        float s = Scale * 0.5f;
        int i = 0;
        
        baseVertices[i++] = new Vector3(-s, -s, -s);
        baseVertices[i++] = new Vector3(s, -s, -s);
        baseVertices[i++] = new Vector3(s, -s, s);
        baseVertices[i++] = new Vector3(-s, -s, s);
        baseVertices[i++] = new Vector3(-s, s, -s);
        baseVertices[i++] = new Vector3(s, s, -s);
        baseVertices[i++] = new Vector3(s, s, s);
        baseVertices[i++] = new Vector3(-s, s, s);

        for (int j = 0; j < baseVertices.Length; j++)
        {
            var v = baseVertices[j];
            float y = Mathf.Sin(v.x * XFactor + v.y * YFactor + v.z * ZFactor + Time.time) * Stretch;
            baseVertices[j] += new Vector3(0, y, 0);
        }
        
        
        Vector3[] vertices = new Vector3[24];
        i = 0;
        // bottom
        vertices[i++] = baseVertices[0];
        vertices[i++] = baseVertices[1];
        vertices[i++] = baseVertices[2];
        vertices[i++] = baseVertices[3];
        // Front
        vertices[i++] = baseVertices[4];
        vertices[i++] = baseVertices[5];
        vertices[i++] = baseVertices[1];
        vertices[i++] = baseVertices[0];
        // Right
        vertices[i++] = baseVertices[5];
        vertices[i++] = baseVertices[6];
        vertices[i++] = baseVertices[2];
        vertices[i++] = baseVertices[1];
        // Left
        vertices[i++] = baseVertices[7];
        vertices[i++] = baseVertices[4];
        vertices[i++] = baseVertices[0];
        vertices[i++] = baseVertices[3];
        // Back
        vertices[i++] = baseVertices[6];
        vertices[i++] = baseVertices[7];
        vertices[i++] = baseVertices[3];
        vertices[i++] = baseVertices[2];
        // Top
        vertices[i++] = baseVertices[7];
        vertices[i++] = baseVertices[6];
        vertices[i++] = baseVertices[5];
        vertices[i++] = baseVertices[4];
        
        i = 0;
        int[] indices = new int[36];
        for (int v = 0; v < 24; v += 4)
        {
            indices[i++] = v + 0; indices[i++] = v + 1; indices[i++] = v + 2;
            indices[i++] = v + 0; indices[i++] = v + 2; indices[i++] = v + 3;
        }
        
        GeneratePlane(Vector3.forward, Vector3.up, Scale, 0, 5);
        
        mesh.SetVertices(vertices);
        mesh.SetIndices(indices, MeshTopology.Triangles, 0);
        
        mesh.RecalculateNormals();    
        mesh.RecalculateBounds();
        
        
        
        meshFilter.mesh = mesh;
    }

    List<Vector3> planePoints = new List<Vector3>();
    
    (List<Vector3>, List<int>) GeneratePlane(Vector3 basisX, Vector3 basisY, float scale, int indexOffset, int resolution)
    {
        planePoints.Clear();
        List<Vector3> vertices = new List<Vector3>();
        List<int> indices = new List<int>();
        
        Vector3 awayFromCenter = -Vector3.Cross(basisX, basisY);
        Vector3 startingPoint = (awayFromCenter - basisX - basisY) * 0.5f * scale; 
        
        int currentIndex = indexOffset;

        for (float y = 0; y <= resolution; y++)
        {
            float yt = y / resolution;
            for (float x = 0; x <= resolution; x++)
            {
                float xt = x / resolution;
                Vector3 point = startingPoint + (yt * basisY + xt * basisX) * Scale;
                planePoints.Add(point);
            }
        }
        return (vertices, indices);
    }

    void OnDrawGizmos()
    {
        foreach (var planePoint in planePoints)
        {
            Gizmos.DrawCube(planePoint, Vector3.one * 0.1f);    
        }
    }
}
