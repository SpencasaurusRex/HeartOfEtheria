using UnityEngine;

public class HeartbeatMorph : MonoBehaviour, IMorph
{
    public float XFactor;
    public float YFactor;
    public float ZFactor;
    public float Stretch;
    
    public Vector3 MorphPoint(Vector3 point)
    {
        float val = Mathf.Abs((((Time.time 
                                 + point.x * XFactor
                                 + point.y * YFactor
                                 + point.z * ZFactor
            ) * 7) % 10) - 2) - 1;
        float g = val * val * -10 + 2;
        float scale = SmoothMin(1, g, -6);
        scale = (scale - 1) * Stretch + 1;
             
        return point * scale;
    }
    
    float SmoothMin(float a, float b, float k)
    {
        float val1 = Mathf.Clamp01((b - a + k) / (2 * k));
        return a * val1 + b * (1 - val1) - k * val1 * (1 - val1);
    }
}
