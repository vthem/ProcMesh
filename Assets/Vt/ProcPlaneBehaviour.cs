using System;
using Unity.Collections;
using UnityEngine;
using SysStopwatch = System.Diagnostics.Stopwatch;

public struct ProcPlaneCreateParameters
{
    public string name;
    public string materialName;
    public Transform parent;
    public MeshLodInfo lodInfo;
    public IVertexModifier vertexModifier;

    public ProcPlaneCreateParameters(string name,
                                     string materialName,
                                     IVertexModifier vertexModifier)
    {
        this.name = name;
        this.materialName = materialName;
        this.vertexModifier = vertexModifier;
        parent = null;
        lodInfo.leftLod = lodInfo.rightLod = lodInfo.frontLod = lodInfo.backLod = -1;
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
        if (procPlane.vertexModifier != null)
        {
            var vertexModifierUnityObject = procPlane.vertexModifier as UnityEngine.Object;
            if (vertexModifierUnityObject)
                Destroy(vertexModifierUnityObject);
        }
        procPlane.lodInfo = createParams.lodInfo;
        procPlane.vertexModifier = createParams.vertexModifier;
        var customVertexModifier = createParams.vertexModifier as VertexModifierScriptableObject;
        procPlane.customVertexModifier = customVertexModifier;

        meshRenderer.sharedMaterial = Resources.Load(createParams.materialName) as Material;
        return procPlane;
    }

    public int LeftLod { get => lodInfo.leftLod; set => lodInfo.leftLod = value; }
    public int FrontLod { get => lodInfo.frontLod; set => lodInfo.frontLod = value; }
    public int RightLod { get => lodInfo.rightLod; set => lodInfo.rightLod = value; }
    public int BackLod { get => lodInfo.backLod; set => lodInfo.backLod = value; }
    public IVertexModifier VertexModifier { get => vertexModifier; set => vertexModifier = value; }

    public T GetVertexModifierAs<T>() where T : class
    {
        return vertexModifier as T;
    }

    #region private
    [SerializeField]
    private MeshLodInfo lodInfo;

    [SerializeField]
    private bool forceRebuild = false;

    [SerializeField]
    private VertexModifierScriptableObject customVertexModifier;

    private IVertexModifier vertexModifier;
    private MeshGenerateParameter meshGenerateParameter;

    static bool benchEnable = false;
    static long benchElaspedMilliseconds = 0;
    static int benchTotalVerticesProcessed = 0;

    private Matrix4x4 objToParent;
    private Material material;
    private bool forceRebuildOnce = false;

    private void Awake()
    {
        if (customVertexModifier)
            vertexModifier = customVertexModifier;
        else
            vertexModifier = ScriptableObject.CreateInstance<VertexModifierScriptableObject>();
    }

    private void Update()
    {
        if (!material)
            material = GetComponent<MeshRenderer>().sharedMaterial;

        material.SetMatrix("_ObjToParent", objToParent);
        if (!IsMeshInfoValid() || forceRebuild || (vertexModifier.HasChanged) || forceRebuildOnce)
        {
            forceRebuildOnce = false;
            if (customVertexModifier)
                customVertexModifier.HasChanged = false;

            ReleaseMeshInfo();
            AllocateMeshInfo();

            objToParent = Matrix4x4.TRS(transform.localPosition, Quaternion.identity, Vector3.one);

            if (benchEnable)
            {
                SysStopwatch sw = SysStopwatch.StartNew();
                ProceduralPlaneMesh.Generate(meshGenerateParameter);
                benchElaspedMilliseconds += sw.ElapsedMilliseconds;
                benchTotalVerticesProcessed += customVertexModifier.VertexCount2D;
            }
            else
            {
                ProceduralPlaneMesh.Generate(meshGenerateParameter);
            }
        }
    }

    private void OnGUI()
    {
        //if (benchEnable && benchElaspedMilliseconds > 0)
        //    GUILayout.Label($"{benchTotalVerticesProcessed / benchElaspedMilliseconds} vertices/ms");
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
        meshGenerateParameter.vertexModifier = vertexModifier;
        meshGenerateParameter.lodInfo = lodInfo;

        meshGenerateParameter.vertices = new NativeArray<ExampleVertex>(vertexModifier.VertexCount2D, Allocator.Persistent);
        meshGenerateParameter.indices = new NativeArray<ushort>(vertexModifier.IndiceCount, Allocator.Persistent);
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
        return meshGenerateParameter.lodInfo == lodInfo;
    }

    private void OnDestroy()
    {
        ReleaseMeshInfo();
    }

    private void OnValidate()
    {
        forceRebuildOnce = true;
    }
    #endregion // private
}
