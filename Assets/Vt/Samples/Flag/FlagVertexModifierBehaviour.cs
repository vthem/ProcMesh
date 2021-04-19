using UnityEngine;

//public class FlagVertexModifier : PerlinVertexModifier
//{
//    public FlagVertexModifier(float xSize, float zSize) : base(xSize, zSize)
//    {
//        xStart = -xSize * 0.5f;
//        zStart = -zSize * 0.5f;
//    }

//    public override Vector3 Execute(int x, int z)
//    {
//        var pScale = this.perlinScale * (x / (float)xCount).Remap(0f, 1f, 0.333f, 1.333f);
//        perlinMatrix = Matrix4x4.TRS(perlinOffset + new Vector3(Time.time * -5f, 0, 0), Quaternion.identity, Vector3.one * pScale) * Matrix4x4.TRS(localPosition, Quaternion.identity, Vector3.one);

//        float xVal = xStart + x * xDelta;
//        float zVal = zStart + z * zDelta;
//        var v = perlinMatrix.MultiplyPoint(new Vector3(xVal, 0, zVal));
//        return new Vector3(xVal, Mathf.PerlinNoise(v.x, v.z) * (x / (float)xCount) * 0.3f, zVal);
//    }
//}

public class FlagVertexModifierBehaviour : MonoBehaviour
{
    //protected override PerlinVertexModifier CreateVertexModifier()
    //{
    //    return new FlagVertexModifier(xSize, zSize);
    //}
}
