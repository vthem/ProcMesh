using UnityEngine;

public static class VertexModifier
{
    public static int ComputeVertexCount1D(int lod) => (int)Mathf.Pow(2, lod) + 1;
}

public class VertexModifierBase :  IVertexModifier
{
    public float XSize { get => xSize; set { (HasChanged, xSize) = xSize.SetValue(value); } }
    public float ZSize { get => zSize; set { (HasChanged, zSize) = zSize.SetValue(value); } }
    public int Lod { get => lod; set { (HasChanged, lod) = lod.SetValue(value); } }

    public virtual bool Initialize()
    {
        xStart = -xSize * 0.5f;
        zStart = -zSize * 0.5f;
        xDelta = xSize / (float)(VertexCount1D - 1);
        zDelta = zSize / (float)(VertexCount1D - 1);
        return true;
    }

    public virtual Vector3 Vertex(int x, int z)
    {
        float xVal = xStart + x * xDelta;
        float zVal = zStart + z * zDelta;
        return new Vector3(xVal, 0, zVal);
    }

    public bool HasChanged { get; set; }

    public int VertexCount1D => VertexModifier.ComputeVertexCount1D(lod);

    public int VertexCount2D
    {
        get
        {
            var vc1d = VertexCount1D;
            return vc1d * vc1d;
        }
    }

    public int IndiceCount
    {
        get
        {
            var vc1d = VertexCount1D;
            return (vc1d - 1) * (vc1d - 1) * 2 * 3;
        }
    }

    protected float xSize = 1f;
    protected float zSize = 1f;
    protected int lod = 0;

    protected float xDelta;
    protected float zDelta;
    protected float xStart;
    protected float zStart;
}

[System.Serializable]
[CreateAssetMenu(fileName = "VertexModifier", menuName = "Vt/ScriptableObjects/VertexModifier", order = 1)]
public class VertexModifierScriptableObject : ScriptableObject, IVertexModifier
{
    public float XSize { get => xSize; set { (HasChanged, xSize) = xSize.SetValue(value); } }
    public float ZSize { get => zSize; set { (HasChanged, zSize) = zSize.SetValue(value); } }
    public int Lod { get => lod; set { (HasChanged, lod) = lod.SetValue(value); } }

    public virtual bool Initialize()
    {
        xStart = -xSize * 0.5f;
        zStart = -zSize * 0.5f;
        xDelta = xSize / (float)(VertexCount1D - 1);
        zDelta = zSize / (float)(VertexCount1D - 1);
        return true;
    }

    public virtual Vector3 Vertex(int x, int z)
    {
        float xVal = xStart + x * xDelta;
        float zVal = zStart + z * zDelta;
        return new Vector3(xVal, 0, zVal);
    }

    public bool HasChanged { get; set; }

    public int VertexCount1D => VertexModifier.ComputeVertexCount1D(lod);

    public int VertexCount2D
    {
        get
        {
            var vc1d = VertexCount1D;
            return vc1d * vc1d;
        }
    }

    public int IndiceCount
    {
        get
        {
            var vc1d = VertexCount1D;
            return (vc1d - 1) * (vc1d - 1) * 2 * 3;
        }
    }

    [SerializeField] protected float xSize = 1f;
    [SerializeField] protected float zSize = 1f;
    [SerializeField] [Range(0, 7)] protected int lod = 0;

    protected float xDelta;
    protected float zDelta;
    protected float xStart;
    protected float zStart;

    #region private
    private void OnValidate()
    {
        HasChanged = true;
    }
    #endregion
}