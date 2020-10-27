using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;


public enum Shape
{
    Cube,
    Plane,
    Sphere,
    WireCube
}

public class ShapeGenerator : MonoBehaviour
{
    MeshFilter meshFilter;
    
    public float Scale = 1;
    [Range(1, 50)]
    public int Resolution = 5;
    [Range(1, 10)]
    public int ColumnWidth = 2;
    
    public Shape Shape;

    List<IMorph> morphs;
    
    void Update()
    {
        Generate();
    }

    void OnValidate()
    {
        morphs = GetComponents<IMorph>().ToList();
        Generate();
    }

    void Generate()
    {
        meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        List<int> indices = new List<int>();

        
        GeneratePlane(Vector3.right, Vector3.up, Scale, Resolution, vertices, indices);
        if (Shape == Shape.Plane)
        {
            // Back side of plane
            GeneratePlane(Vector3.left, Vector3.up, Scale, Resolution, vertices, indices);
        }
        else
        {
            GeneratePlane(Vector3.forward, Vector3.up, Scale, Resolution, vertices, indices);
            GeneratePlane(Vector3.left, Vector3.up, Scale, Resolution, vertices, indices);
            GeneratePlane(Vector3.back, Vector3.up, Scale, Resolution, vertices, indices);
            GeneratePlane(Vector3.right, Vector3.forward, Scale, Resolution, vertices, indices);
            GeneratePlane(Vector3.right, Vector3.back, Scale, Resolution, vertices, indices);
        }

        if (Shape == Shape.WireCube)
        {
            GenerateColumnSide(Vector3.forward, Vector3.up, Scale, Resolution, vertices, indices);
        }
        
        if (morphs == null)
        {
            morphs = GetComponents<IMorph>().ToList();
        }
        
        for (int j = 0; j < vertices.Count; j++)
        {
             var v = vertices[j];

             if (Shape == Shape.Sphere)
             {
                 vertices[j] = vertices[j].normalized * Scale;
             }

             for (int i = 0; i < morphs.Count; i++)
             {
                 vertices[j] = morphs[i].MorphPoint(vertices[j]);
             }
        }

        mesh.SetVertices(vertices);
        mesh.SetIndices(indices, MeshTopology.Triangles, 0);

        mesh.RecalculateNormals();    
        mesh.RecalculateBounds();
        
        meshFilter.mesh = mesh;
    }



    
    void GeneratePlane(Vector3 basisX, Vector3 basisY, float scale, int resolution, List<Vector3> vertices, List<int> indices)
    {
        Vector3 awayFromCenter = -Vector3.Cross(basisX, basisY);
        
        Vector3 startingPoint = ((Shape == Shape.Plane ? Vector3.zero : awayFromCenter) 
                                 - basisX - basisY) 
                                * 0.5f * scale;

        int currentIndex = vertices.Count;

        for (float y = 0; y <= resolution; y++)
        {
            float yt = y / resolution;
            for (float x = 0; x <= resolution; x++)
            {
                float xt = x / resolution;
                Vector3 point = startingPoint + (yt * basisY + xt * basisX) * Scale;
                vertices.Add(point);
            }
        }
        
        int c = ColumnWidth - 1;
        var w = resolution + 1;
        for (int j = 0; j < resolution; j++)
        {
            for (int i = 0; i < resolution; i++)
            {
                if (Shape == Shape.WireCube && i > c && i < resolution - 1 - c && j > c && j < resolution - 1 - c) continue;
                indices.Add(currentIndex + 0 + i + j * w);
                indices.Add(currentIndex + w + 1 + i + j * w);
                indices.Add(currentIndex + 1 + i + j * w);
                indices.Add(currentIndex + 0 + i + j * w);
                indices.Add(currentIndex + w + i + j * w);
                indices.Add(currentIndex + w + 1 + i + j * w);    
            }            
        }
    }

    List<Vector3> points = new List<Vector3>();
    void GenerateColumnSide(Vector3 basisX, Vector3 basisY, float scale, int resolution, List<Vector3> vertices, List<int> indices)
    {
        points.Clear();
        float triangleWidth = scale / resolution;
        
        Vector3 awayFromCenter = -Vector3.Cross(basisX, basisY);
        Vector3 basisZ = -awayFromCenter;
        
        int currentIndex = vertices.Count;
        
        Vector3 startingPoint = ((Shape == Shape.Plane ? Vector3.zero : awayFromCenter) 
                                 - basisX - basisY) 
                                * 0.5f * scale;
        
        startingPoint += (basisX + basisY) * ColumnWidth * triangleWidth;

        for (int z = 0; z <= ColumnWidth; z++)
        {
            for (int x = 0; x <= resolution - ColumnWidth * 2; x++)
            {
                vertices.Add(startingPoint + (basisX * x + basisZ * z) * triangleWidth);
                points.Add(startingPoint + (basisX * x + basisZ * z) * triangleWidth);
            }
        }
        
        
        
        int h = resolution - 2 * ColumnWidth + 1;

        // indices.Add(currentIndex + 0);
        // indices.Add(currentIndex + h);
        // indices.Add(currentIndex + 1);
        
        // points.Clear();
        // points.Add(vertices[currentIndex]);
        // points.Add(vertices[currentIndex + h]);
        // points.Add(vertices[currentIndex + 1]);

        for (int z = 0; z < ColumnWidth; z++)
        {
            for (int x = 0; x < resolution - ColumnWidth * 2; x++)
            {
                indices.Add(currentIndex + 0 + x + z * h);
                indices.Add(currentIndex + h + x + z * h);
                indices.Add(currentIndex + 1 + x + z * h);
                indices.Add(currentIndex + 1 + x + z * h);
                indices.Add(currentIndex + h + x + z * h);
                indices.Add(currentIndex + h + 1 + x + z * h);    
            }    
        }
    }

    void OnDrawGizmos()
    {
        foreach (var point in points)
        {
            Gizmos.DrawSphere(point, 0.01f);
        }
    }
}
