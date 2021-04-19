using System;
using UnityEngine;

//public class PerlinVertexModifier : VertexModifier
//{
//    public PerlinVertexModifier(float xSize, float zSize) : base(xSize, zSize)
//    {
//        xStart = -xSize * 0.5f;
//        zStart = -zSize * 0.5f;
//    }

//    public void Update()
//    {
//        perlinMatrix = Matrix4x4.TRS(perlinOffset, Quaternion.identity, Vector3.one * this.PerlinScale) * Matrix4x4.TRS(localPosition, Quaternion.identity, Vector3.one);
//    }

//    public override Vector3 Execute(int x, int z)
//    {
//        float xVal = xStart + x * xDelta;
//        float zVal = zStart + z * zDelta;
//        var v = perlinMatrix.MultiplyPoint(new Vector3(xVal, 0, zVal));
//        return new Vector3(xVal, Mathf.PerlinNoise(v.x, v.z), zVal);
//    }

//    public override void Initialize(int xCount, int zCount)
//    {
//        base.Initialize(xCount, zCount);

//        xDelta = xSize / (float)(xCount - 1);
//        zDelta = zSize / (float)(zCount - 1);
//    }

//    public Vector3 PerlinOffset { get => perlinOffset; set { (HasChanged, perlinOffset) = perlinOffset.SetValue(value); } }
//    public float PerlinScale { get => perlinScale; set { (HasChanged, perlinScale) = perlinScale.SetValue(value); ; } }
//    public Vector3 LocalPosition { get => localPosition; set { (HasChanged, localPosition) = perlinOffset.SetValue(value); } }

//    protected Vector3 perlinOffset = Vector3.one;
//    protected float perlinScale = 1f;
//    protected Vector3 localPosition;
//    protected Matrix4x4 perlinMatrix = Matrix4x4.identity;

//    protected float xDelta = 0f;
//    protected float zDelta = 0f;
//    protected float xStart = 0f;
//    protected float zStart = 0f;
//}

[System.Serializable]
public struct PerlinVertexModifier
{
    public float xSize;
    public float zSize;
    public Vector3 perlinOffset;
    public float perlinScale;
    public Vector3 localPosition;

    [NonSerialized] public int xCount;
    [NonSerialized] public int zCount;

    private Matrix4x4 perlinMatrix;
    private float xDelta;
    private float zDelta;
    private float xStart;
    private float zStart;

    public void Update()
    {
        perlinMatrix = Matrix4x4.TRS(perlinOffset, Quaternion.identity, Vector3.one * perlinScale) * Matrix4x4.TRS(localPosition, Quaternion.identity, Vector3.one);
        xStart = -xSize * 0.5f;
        zStart = -zSize * 0.5f;
        xDelta = xSize / (float)(xCount - 1);
        zDelta = zSize / (float)(zCount - 1);
    }

    public Vector3 VertexFunc(int x, int z)
    {
        float xVal = xStart + x * xDelta;
        float zVal = zStart + z * zDelta;
        var v = perlinMatrix.MultiplyPoint(new Vector3(xVal, 0, zVal));
        return new Vector3(xVal, Mathf.PerlinNoise(v.x, v.z), zVal);
    }

    public static PerlinVertexModifier Default
    {
        get
        {
            var d = new PerlinVertexModifier();
            d.xSize = 1f;
            d.zSize = 1f;
            d.perlinOffset = Vector3.one;
            d.perlinScale = 1f;
            return d;
        }
    }
}

public class PerlinVertexModifierBehaviour : MonoBehaviour
{
    #region private
    protected ProcPlaneBehaviour procPlane;

    [SerializeField]
    protected PerlinVertexModifier perlinVertexModifier = PerlinVertexModifier.Default;

    private void Awake()
    {
        procPlane = GetComponent<ProcPlaneBehaviour>();
        if (!procPlane)
        {
            Debug.LogError($"{nameof(PerlinVertexModifierBehaviour)} on object {name} requires a {nameof(ProcPlaneBehaviour)}");
            enabled = false;
            return;
        }

        procPlane.VertexModifierProvider = GetVertexModifier;
    }

    private void Update()
    {
        perlinVertexModifier.Update();
    }

    VertexModifier GetVertexModifier()
    {
        perlinVertexModifier.Update();
        return perlinVertexModifier.VertexFunc;
    }

    #endregion // private
}
