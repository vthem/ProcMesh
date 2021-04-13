using Unity.Collections;
using UnityEngine;
using SysStopwatch = System.Diagnostics.Stopwatch;

public struct ProcPlaneCreateParameters
{
    public string name;
    public string materialName;
    public Transform parent;
    public MeshInfo meshInfo;
    public ProcPlaneBehaviourVertexModifier vertexModifier;

    public ProcPlaneCreateParameters(string name,
                                     int lod,
                                     Vector2 size,
                                     string materialName,
                                     ProcPlaneBehaviourVertexModifier vertexModifier)
    {
        this.name = name;
        this.materialName = materialName;
        this.vertexModifier = vertexModifier;
        meshInfo.lod = lod;
        meshInfo.size = size;
        parent = null;
        meshInfo.leftLod = meshInfo.rightLod = meshInfo.frontLod = meshInfo.backLod = -1;
    }
}

public class ProcPlaneBehaviourVertexModifier : VertexModifier
{
    public bool HasChanged { get; set; }
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
        procPlane.vertexModifier = createParams.vertexModifier;

        meshRenderer.sharedMaterial = Resources.Load(createParams.materialName) as Material;
        return procPlane;
    }

    public int LeftLod { get => meshInfo.leftLod; set => meshInfo.leftLod = value; }
    public int FrontLod { get => meshInfo.frontLod; set => meshInfo.frontLod = value; }
    public int RightLod { get => meshInfo.rightLod; set => meshInfo.rightLod = value; }
    public int BackLod { get => meshInfo.backLod; set => meshInfo.backLod = value; }
    public int Lod { get => meshInfo.lod; set => meshInfo.lod = value; }
    public ProcPlaneBehaviourVertexModifier VertexModifier { get => vertexModifier; set => vertexModifier = value; }

    #region private
    [SerializeField]
    private MeshInfo meshInfo;

    [SerializeField]
    private bool forceRebuild = false;

    private MeshGenerateParameter meshGenerateParameter;

    static bool benchEnable = false;
    static long benchElaspedMilliseconds = 0;
    static int benchTotalVerticesProcessed = 0;

    private Matrix4x4 objToParent;
    private Material material;
    private ProcPlaneBehaviourVertexModifier vertexModifier;

    private void Update()
    {
        if (vertexModifier == null)
        {
            enabled = false;
            Debug.LogError($"{nameof(ProcPlaneBehaviour)} disabled on object {name} because vertexModifier is not set");
            return;
        }

        if (!material)
            material = GetComponent<MeshRenderer>().sharedMaterial;

        GetComponent<Renderer>().material.SetMatrix("_ObjToParent", objToParent);
        if (!IsMeshInfoValid() || forceRebuild || vertexModifier.HasChanged)
        {
            vertexModifier.HasChanged = false;
            
            ReleaseMeshInfo();
            AllocateMeshInfo();

            objToParent = Matrix4x4.TRS(transform.localPosition, Quaternion.identity, Vector3.one);
            meshGenerateParameter.vertexModifier = vertexModifier;

            if (benchEnable)
            {
                SysStopwatch sw = SysStopwatch.StartNew();
                ProcPlane.Gen6(meshGenerateParameter);
                benchElaspedMilliseconds += sw.ElapsedMilliseconds;
                benchTotalVerticesProcessed += meshGenerateParameter.VertexCount2D;
            }
            else
            {
                ProcPlane.Gen6(meshGenerateParameter);
            }
        }
    }

    private void OnGUI()
    {
        if (benchEnable && benchElaspedMilliseconds > 0)
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
        if (!meshGenerateParameter.mesh || !meshGenerateParameter.indices.IsCreated || !meshGenerateParameter.vertices.IsCreated)
            return false;
        return meshGenerateParameter.meshInfo == meshInfo;
    }

    private void OnDestroy()
    {
        ReleaseMeshInfo();
    }
    #endregion // private
}
