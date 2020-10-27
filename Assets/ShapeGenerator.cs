using System.Collections.Generic;
using UnityEngine;

public class ShapeGenerator : MonoBehaviour
{

    MeshFilter meshFilter;
    
    public float Scale = 1;
    public float Stretch = 1;
    public float XFactor = 1;
    public float YFactor = 1;
    public float ZFactor = 1;
    [Range(1, 50)]
    public int Resolution = 5;
    

    public bool Sphere;
    public bool Plane;
    
    void Update()
    {
        Generate();
    }

    void OnValidate()
    {
        Generate();
    }

    void Generate()
    {
        meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        List<int> indices = new List<int>();

        
        GeneratePlane(Vector3.right, Vector3.up, Scale, Resolution, vertices, indices);
        if (!Plane)
        {
            GeneratePlane(Vector3.forward, Vector3.up, Scale, Resolution, vertices, indices);
            GeneratePlane(Vector3.left, Vector3.up, Scale, Resolution, vertices, indices);
            GeneratePlane(Vector3.back, Vector3.up, Scale, Resolution, vertices, indices);
            GeneratePlane(Vector3.right, Vector3.forward, Scale, Resolution, vertices, indices);
            GeneratePlane(Vector3.right, Vector3.back, Scale, Resolution, vertices, indices);
        }

        // Freaky wave cube
        // for (int j = 0; j < vertices.Count; j++)
        // {
        //     var v = vertices[j];
        //     float y = Mathf.Sin(v.x * XFactor + v.y * YFactor + v.z * ZFactor + Time.time) * Stretch;
        //     vertices[j] += new Vector3(0, y, 0);
        // }

        for (int j = 0; j < vertices.Count; j++)
        {
             var v = vertices[j];

             if (Sphere)
             {
                 vertices[j] = vertices[j].normalized * Scale;
             }
             
             float val = Mathf.Abs((((Time.time 
                                      + v.x * XFactor
                                      + v.y * YFactor
                                      + v.z * ZFactor
                 ) * 7) % 10) - 2) - 1;
             float g = val * val * -10 + 2;
             float scale = SmoothMin(1, g, -6);
             scale = (scale - 1) * Stretch + 1;
             
             vertices[j] = vertices[j] * scale;
        }

        mesh.SetVertices(vertices);
        mesh.SetIndices(indices, MeshTopology.Triangles, 0);

        mesh.RecalculateNormals();    
        mesh.RecalculateBounds();
        
        meshFilter.mesh = mesh;
    }

    float SmoothMin(float a, float b, float k)
    {
        float val1 = Mathf.Clamp01((b - a + k) / (2 * k));
        return a * val1 + b * (1 - val1) - k * val1 * (1 - val1);
    }
    
    void GeneratePlane(Vector3 basisX, Vector3 basisY, float scale, int resolution, List<Vector3> vertices, List<int> indices)
    {
        Vector3 awayFromCenter = -Vector3.Cross(basisX, basisY);
        Vector3 startingPoint = (awayFromCenter - basisX - basisY) * 0.5f * scale; 
        
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
        
        var w = resolution + 1;
        for (int j = 0; j < resolution; j++)
        {
            for (int i = 0; i < resolution; i++)
            {
                indices.Add(currentIndex + 0 + i + j * w);
                indices.Add(currentIndex + w + 1 + i + j * w);
                indices.Add(currentIndex + 1 + i + j * w);
                indices.Add(currentIndex + 0 + i + j * w);
                indices.Add(currentIndex + w + i + j * w);
                indices.Add(currentIndex + w + 1 + i + j * w);    
            }            
        }
    }
}
