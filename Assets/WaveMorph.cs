using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveMorph : MonoBehaviour, IMorph
{
    public float XFactor;
    public float YFactor;
    public float ZFactor;
    public float Stretch;
    
    public Vector3 MorphPoint(Vector3 point)
    {
        float y = Mathf.Sin(point.x * XFactor + point.y * YFactor + point.z * ZFactor + Time.time) * Stretch;
        return point + new Vector3(0, y, 0);
    }
}