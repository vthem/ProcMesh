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

    public int LeftLod { get => meshInfo.leftLod; set => meshInfo.leftLod = value; }
    public int FrontLod { get => meshInfo.frontLod; set => meshInfo.frontLod = value; }
    public int RightLod { get => meshInfo.rightLod; set => meshInfo.rightLod = value; }
    public int BackLod { get => meshInfo.backLod; set => meshInfo.backLod = value; }
    public int Lod { get => meshInfo.lod; set => meshInfo.lod = value; }
    public float PerlinScale { get => perlinScale; set { perlinScale = value; perlinScaleChanged = true; } }
    public Vector3 PerlinOffset { get => perlinOffset; set { perlinOffset = value; perlinOffsetChanged = true; } }

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
    private Vector3 prevLocalPosition;

    [SerializeField]
    private float perlinScale = 1f;
    private bool perlinScaleChanged = false;

    [SerializeField]
    private Vector3 perlinOffset = Vector3.one;
    private bool perlinOffsetChanged = false;

    private Matrix4x4 perlinMatrix;

    private void Update()
    {
        if (!material)
            material = GetComponent<MeshRenderer>().sharedMaterial;

        GetComponent<Renderer>().material.SetMatrix("_ObjToParent", objToParent);
        if (!IsMeshInfoValid() || forceRebuild)
        {
            perlinOffsetChanged = perlinScaleChanged = false;
            
            ReleaseMeshInfo();
            AllocateMeshInfo();

            prevLocalPosition = transform.localPosition;
            objToParent = Matrix4x4.TRS(transform.localPosition, Quaternion.identity, Vector3.one);

            perlinMatrix = Matrix4x4.TRS(perlinOffset, Quaternion.identity, Vector3.one * perlinScale) * Matrix4x4.TRS(transform.localPosition, Quaternion.identity, Vector3.one);

            if (benchEnable)
            {
                SysStopwatch sw = SysStopwatch.StartNew();
                ProcPlane.Gen6(meshGenerateParameter, Perlin);
                benchElaspedMilliseconds += sw.ElapsedMilliseconds;
                benchTotalVerticesProcessed += meshGenerateParameter.VertexCount2D;
            }
            else
            {
                ProcPlane.Gen6(meshGenerateParameter, Perlin);
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
        if (transform.localPosition != prevLocalPosition)
            return false;
        if (!meshGenerateParameter.mesh || !meshGenerateParameter.indices.IsCreated || !meshGenerateParameter.vertices.IsCreated || !perlinOffsetChanged || !perlinScaleChanged)
            return false;
        return meshGenerateParameter.meshInfo == meshInfo;
    }

    private float Perlin(float x, float z)
    {        
        var v = perlinMatrix.MultiplyPoint(new Vector3(x, 0, z));
        return Mathf.PerlinNoise(v.x, v.z);
    }

    private void OnDestroy()
    {
        ReleaseMeshInfo();
    }
    #endregion // private
}
