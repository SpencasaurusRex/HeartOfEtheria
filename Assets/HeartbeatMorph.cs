using System.Collections.Generic;
using UnityEngine;

public class HeartbeatMorph : MonoBehaviour, IMorph
{
    public float XFactor;
    public float YFactor;
    public float ZFactor;
    public float Stretch;
    
    void OnValidate()
    {
        var gen = GetComponent<ShapeGenerator>();
        gen.GenerateBaseShape();
        gen.UpdateMorphs();
        gen.Morph();
    }
    
    public void MorphPoints(List<Vector3> points)
    {
        for (int i = 0; i < points.Count; i++)
        {
            var point = points[i];
            float val = Mathf.Abs((((Time.time 
                                     + point.x * XFactor
                                     + point.y * YFactor
                                     + point.z * ZFactor
                ) * 7) % 10) - 2) - 1;
            float g = val * val * -10 + 2;
            float scale = SmoothMin(1, g, -6);
            scale = (scale - 1) * Stretch + 1;
            points[i] = point * scale;
        }
    }
    
    float SmoothMin(float a, float b, float k)
    {
        float val1 = Mathf.Clamp01((b - a + k) / (2 * k));
        return a * val1 + b * (1 - val1) - k * val1 * (1 - val1);
    }
}
