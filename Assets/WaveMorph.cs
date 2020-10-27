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

    public Vector3 MorphPoint(Vector3 point)
    {
        float y = Mathf.Sin(point.x * XFactor + point.y * YFactor + point.z * ZFactor + Time.time) * Stretch;
        return point + new Vector3(0, y, 0);
    }
}