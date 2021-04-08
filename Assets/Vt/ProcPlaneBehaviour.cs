using Unity.Collections;
using UnityEngine;
using SysStopwatch = System.Diagnostics.Stopwatch;

public struct ProcPlaneCreateParameters
{
    public string name;
    public string materialName;
    public Transform parent;
    public MeshInfo meshInfo;

    public ProcPlaneCreateParameters(string name, int lod, Vector2 size, string materialName)
    {
        this.name = name;
        this.materialName = materialName;
        meshInfo.lod = lod;
        meshInfo.size = size;
        parent = null;
        meshInfo.leftLod = meshInfo.rightLod = meshInfo.frontLod = meshInfo.backLod = -1;
    }
}

public class ProcPlaneBehaviour : MonoBehaviour
{
    public static ProcPlaneBehaviour Create(ProcPlaneCreateParameters createParams)
    {
        var obj = new GameObject(createParams.name);
        if (createParams.parent)
            obj.transform.SetParent(createParams.parent);
        var meshFilter = obj.AddComponent<MeshFilter>();
        var meshRenderer = obj.AddComponent<MeshRenderer>();
        obj.name = createParams.name;

        var mesh = meshFilter.sharedMesh;
        if (!mesh)
        {
            mesh = new Mesh();
            meshFilter.sharedMesh = mesh;
        }
        mesh.MarkDynamic();

        var procPlane = obj.AddComponent<ProcPlaneBehaviour>();
        procPlane.meshInfo = createParams.meshInfo;

        meshRenderer.sharedMaterial = Resources.Load(createParams.materialName) as Material;
        return procPlane;
    }

    #region private
    [SerializeField]
    private MeshInfo meshInfo;
    private MeshGenerateParameter meshGenerateParameter;

    static long benchElaspedMilliseconds = 0;
    static int benchTotalVerticesProcessed = 0;

    private void Update()
    {
        if (!IsMeshInfoValid())
        {
            transform.hasChanged = false;
            ReleaseMeshInfo();
            AllocateMeshInfo();
            SysStopwatch sw = SysStopwatch.StartNew();
            ProcPlane.Gen6(meshGenerateParameter, Perlin);
            benchElaspedMilliseconds += sw.ElapsedMilliseconds;
            benchTotalVerticesProcessed += meshGenerateParameter.VertexCount2D;
        }
    }

    private void OnGUI()
    {
        if (benchElaspedMilliseconds > 0)
            GUILayout.Label($"{benchTotalVerticesProcessed / benchElaspedMilliseconds} vertices/ms");
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
        meshGenerateParameter.mesh = mesh;
        meshGenerateParameter.meshInfo = meshInfo;

        meshGenerateParameter.vertices = new NativeArray<ExampleVertex>(meshGenerateParameter.VertexCount2D, Allocator.Persistent);
        meshGenerateParameter.indices = new NativeArray<ushort>(meshGenerateParameter.IndiceCount, Allocator.Persistent);
    }

    private void ReleaseMeshInfo()
    {
        if (meshGenerateParameter.vertices.IsCreated)
        {
            meshGenerateParameter.vertices.Dispose();
        }
        if (meshGenerateParameter.indices.IsCreated)
        {
            meshGenerateParameter.indices.Dispose();
        }
    }

    private bool IsMeshInfoValid()
    {
        if (transform.hasChanged)
            return false;
        if (!meshGenerateParameter.mesh || !meshGenerateParameter.indices.IsCreated || !meshGenerateParameter.vertices.IsCreated)
            return false;
        return meshGenerateParameter.meshInfo == meshInfo;
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
