using Unity.Collections;
using UnityEngine;

public struct ProcPlaneCreateInfo
{
    public string mName;
    public int mLod;
    public Vector2 mSize;
    public Transform oParent;
}

public class ProcPlaneBehaviour : MonoBehaviour
{
    public static ProcPlaneBehaviour Create(ProcPlaneCreateInfo createInfo)
    {
        var name = createInfo.mName;
        var lod = createInfo.mLod;
        var size = createInfo.mSize;
        var parent = createInfo.oParent;

        var obj = new GameObject(name);
        if (parent)
            obj.transform.SetParent(parent);
        var meshFilter = obj.AddComponent<MeshFilter>();
        var meshRenderer = obj.AddComponent<MeshRenderer>();
        obj.name = name;

        var mesh = meshFilter.sharedMesh;
        if (!mesh)
        {
            mesh = new Mesh();
            meshFilter.sharedMesh = mesh;
        }
        mesh.MarkDynamic();

        var procPlane = obj.AddComponent<ProcPlaneBehaviour>();
        procPlane.lod = lod;
        procPlane.size = size;

        meshRenderer.sharedMaterial = Resources.Load("Wireframe") as Material;
        return procPlane;
    }

    #region private
    [Range(0, 10)]
    [SerializeField]
    private int lod = 0;

    [SerializeField]
    private Vector2 size = Vector2.one;

    private MeshInfo meshInfo;

    private void Update()
    {
        if (!IsMeshInfoValid())
        {
            transform.hasChanged = false;
            ReleaseMeshInfo();
            AllocateMeshInfo();
            ProcPlane.Gen6(meshInfo, Perlin);
        }
    }

    private void AllocateMeshInfo()
    {
        var mesh = GetComponent<MeshFilter>().sharedMesh;
        if (!mesh)
        {
            mesh = new Mesh();
            GetComponent<MeshFilter>().sharedMesh = mesh;
        }
        mesh.MarkDynamic();
        meshInfo.mesh = mesh;
        meshInfo.size = size;
        meshInfo.lod = lod;

        meshInfo.vertices = new NativeArray<ExampleVertex>(meshInfo.VertexCount2D, Allocator.Persistent);
        meshInfo.indices = new NativeArray<ushort>(meshInfo.IndiceCount, Allocator.Persistent);
    }

    private void ReleaseMeshInfo()
    {
        if (meshInfo.vertices.IsCreated)
        {
            meshInfo.vertices.Dispose();
        }
        if (meshInfo.indices.IsCreated)
        {
            meshInfo.indices.Dispose();
        }
    }

    private bool IsMeshInfoValid()
    {
        if (transform.hasChanged)
            return false;
        if (!meshInfo.mesh || !meshInfo.indices.IsCreated || !meshInfo.vertices.IsCreated)
            return false;
        return meshInfo.size == size && meshInfo.lod == lod;
    }

    private float Perlin(float x, float z)
    {
        var v = transform.TransformPoint(new Vector3(x, 0, z));
        return Mathf.PerlinNoise(v.x, v.z);
    }

    private void OnDestroy()
    {
        ReleaseMeshInfo();
    }
    #endregion // private
}
