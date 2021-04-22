using UnityEngine;

[CreateAssetMenu(fileName = "FlagPerlinVertexModifier", menuName = "Vt/ScriptableObjects/FlagPerlinVertexModifier", order = 1)]
public class FlagPerlinVertexModifierScriptableObject : PerlinVertexModifierScriptableObject
{
    [SerializeField][Range(1f, 5f)] protected float windForce = 5f;
    [SerializeField] [Range(0f, 5f)] protected float yScale = 1f;
    [SerializeField] protected AnimationCurve xPerlinScale;

    public override Vector3 Vertex(int x, int z)
    {
        var pScale = xPerlinScale.Evaluate(x / (float)(VertexCount1D-1));
        perlinMatrix = Matrix4x4.TRS(perlinOffset + new Vector3(Time.time * -windForce, 0, 0), Quaternion.identity, Vector3.one * pScale);
        float xVal = xStart + x * xDelta;
        float zVal = zStart + z * zDelta;
        var v = perlinMatrix.MultiplyPoint(new Vector3(xVal, 0, zVal));
        return new Vector3(xVal, Mathf.PerlinNoise(v.x, v.z) * (x / (float)VertexCount1D) * yScale, zVal);
    }
}
