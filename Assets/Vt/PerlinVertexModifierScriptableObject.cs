using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PerlinVertexModifier", menuName = "Vt/ScriptableObjects/PerlinVertexModifier", order = 1)]
public class PerlinVertexModifierScriptableObject : VertexModifierScriptableObject
{
    public override bool Initialize()
    {
        if (!base.Initialize())
            return false;

        perlinMatrix = Matrix4x4.TRS(perlinOffset, Quaternion.identity, Vector3.one * perlinScale) * Matrix4x4.TRS(localPosition, Quaternion.identity, Vector3.one);

        return true;
    }

    public override Vector3 Vertex(int x, int z)
    {
        float xVal = xStart + x * xDelta;
        float zVal = zStart + z * zDelta;
        var v = perlinMatrix.MultiplyPoint(new Vector3(xVal, 0, zVal));
        return new Vector3(xVal, Mathf.PerlinNoise(v.x, v.z), zVal);
    }

    #region private
    [SerializeField] protected Vector3 perlinOffset;
    [SerializeField] protected float perlinScale;
    [SerializeField] protected Vector3 localPosition;

    protected Matrix4x4 perlinMatrix;
    #endregion // private
}
