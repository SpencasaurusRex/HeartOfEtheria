using System.Collections.Generic;
using UnityEngine;

public class HeartbeatMorph : MonoBehaviour, IMorph
{
    public float XFactor;
    public float YFactor;
    public float ZFactor;
    public float TimeFactor;
    public float Stretch;
    
    public float RepeatTime;
    public float Width;
    public float Sharpness;
    
    public HeartBeatType Type;
    
    public enum HeartBeatType
    {
        Type1,
        Type2
    }
    
    void OnValidate()
    {
        var gen = GetComponent<ShapeGenerator>();
        gen.GenerateBaseShape();
        gen.UpdateMorphs();
        gen.Morph();
    }
    
    public void MorphPoints(List<Vector3> points)
    {
        if (Type == HeartBeatType.Type1)
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
        else
        {
            for (int i = 0; i < points.Count; i++)
            {
                var point = points[i];
                float input = Time.time * TimeFactor 
                              + point.x * XFactor
                              + point.y * YFactor
                              + point.z * ZFactor;
                float total = Stretch / (1 + Mathf.Exp(-Sharpness * (input % RepeatTime))) -
                              Stretch / (1 + Mathf.Exp(-Sharpness * (input % RepeatTime - Width))) + 1;
                
                points[i] = point * total;
            }
        }
    }
    
    float SmoothMin(float a, float b, float k)
    {
        float val1 = Mathf.Clamp01((b - a + k) / (2 * k));
        return a * val1 + b * (1 - val1) - k * val1 * (1 - val1);
    }
}
