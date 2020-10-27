using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    
    List<Vector3> baseVertices = new List<Vector3>();
    List<int> baseIndices = new List<int>();
    List<Vector3> morphedVertices = new List<Vector3>();
    
    void Update()
    {
        Morph();
    }

    void OnValidate()
    {
        morphs = GetComponents<IMorph>().ToList();
        GenerateBaseShape();
        Morph();
    }

    public void GenerateBaseShape()
    {
        baseVertices.Clear();
        baseIndices.Clear();
        morphedVertices.Clear();
        
        GeneratePlane(Vector3.right, Vector3.up, Scale, Resolution, baseVertices, baseIndices);
        if (Shape == Shape.Plane)
        {
            // Back side of plane
            GeneratePlane(Vector3.left, Vector3.up, Scale, Resolution, baseVertices, baseIndices);
        }
        else
        {
            GeneratePlane(Vector3.forward, Vector3.up, Scale, Resolution, baseVertices, baseIndices);
            GeneratePlane(Vector3.left, Vector3.up, Scale, Resolution, baseVertices, baseIndices);
            GeneratePlane(Vector3.back, Vector3.up, Scale, Resolution, baseVertices, baseIndices);
            GeneratePlane(Vector3.right, Vector3.forward, Scale, Resolution, baseVertices, baseIndices);
            GeneratePlane(Vector3.right, Vector3.back, Scale, Resolution, baseVertices, baseIndices);
        }

        if (Shape == Shape.WireCube)
        {
            GenerateColumnSide(Vector3.forward, Vector3.up, Scale, Resolution, baseVertices, baseIndices);
        }
        else if (Shape == Shape.Sphere)
        {
            for (int i = 0; i < baseVertices.Count; i++)
            {
                baseVertices[i] = baseVertices[i].normalized;                
            }
        }
        
        for (int i = 0; i < baseVertices.Count; i++)
        {
            morphedVertices.Add(Vector3.zero);
        }
    }

    public void UpdateMorphs()
    {
        morphs = GetComponents<IMorph>().ToList();
    }
    
    public void Morph()
    {
        if (morphs == null)
        {
            UpdateMorphs();   
        }
        
        meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();

        for (int j = 0; j < baseVertices.Count; j++)
        {
             var v = baseVertices[j];

             for (int i = 0; i < morphs.Count; i++)
             {
                 morphedVertices[j] = morphs[i].MorphPoint(v);
             }
        }

        mesh.SetVertices(morphedVertices);
        mesh.SetIndices(baseIndices, MeshTopology.Triangles, 0);

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

    void OnDrawGizmosSelected()
    {
        foreach (var point in points)
        {
            Gizmos.DrawSphere(point, 0.01f);
        }
    }
}
