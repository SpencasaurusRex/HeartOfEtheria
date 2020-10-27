using System.Collections.Generic;
using UnityEngine;

public class WaveMorph : MonoBehaviour, IMorph
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
            float y = Mathf.Sin(point.x * XFactor + point.y * YFactor + point.z * ZFactor + Time.time) * Stretch;    
            points[i] =  point + new Vector3(0, y, 0);
        }
    }
}