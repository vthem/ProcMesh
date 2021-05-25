using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CylinderBezierVertexModifier : VertexModifierBase
{
    public Matrix4x4 localToWorld;
    public float radius = 1f;
    public float sphereRatio = 1f;
    public BezierSegment segment;

    public override Vector3 Vertex(int x, int z)
    {
        float vCount = (float)(VertexCount1D - 1);
        float angle = Mathf.PI * 2 * x / vCount;
        float t = z / vCount;
        Vector3 pOnPath = segment.GetPoint(t);
        Vector3 tan = segment.GetFirstDerivative(t);
        Quaternion q = Quaternion.FromToRotation(Vector3.forward, tan);
        Vector3 pOnCircle = q * new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
        return pOnPath + pOnCircle;
    }
}

public class CylinderBezierSegmentBehaviour : MonoBehaviour
{
    [SerializeField] private string meshMaterialName = "Grid";
    [SerializeField] private float radius = 10f;
    [SerializeField] [Range(0, 7)] private int lod = 1;


    private ProcPlaneBehaviour[] procPlanes;
    private bool forceUpdate = false;
    private BezierSegmentBehaviour segmentBehaviour;

    public BezierSegment Segment
    {
        get
        {
            if (!segmentBehaviour)
                segmentBehaviour = this.SafeGetComponent<BezierSegmentBehaviour>();
            return segmentBehaviour.segment;
        }
        set
        {
            if (!segmentBehaviour)
                segmentBehaviour = this.GetComponent<BezierSegmentBehaviour>();
            segmentBehaviour.segment = value;
        }
    }

    private void Start()
    {
        procPlanes = new ProcPlaneBehaviour[1];
        VertexModifierBase vm = new CylinderBezierVertexModifier();
        vm.Lod = lod;
        ProcPlaneCreateParameters createInfo = new ProcPlaneCreateParameters(
            name: "CylinderBezier",
            materialName: meshMaterialName,
            vertexModifier: vm
        );
        var procPlane = ProcPlaneBehaviour.Create(createInfo);
        procPlanes[0] = procPlane;
    }

    private void Update()
    {
        if (forceUpdate)
        {
            for (int i = 0; i < procPlanes.Length; ++i)
            {
                procPlanes[i].GetVertexModifierAs<CylinderBezierVertexModifier>().Lod = lod;
                //procPlanes[i].GetVertexModifierAs<CylinderBezierVertexModifier>().XSize = Mathf.Max(1, radius);
                //procPlanes[i].GetVertexModifierAs<CylinderBezierVertexModifier>().ZSize = Mathf.Max(1, radius);
                procPlanes[i].GetVertexModifierAs<CylinderBezierVertexModifier>().radius = radius;
                procPlanes[i].GetVertexModifierAs<CylinderBezierVertexModifier>().segment = Segment;
                procPlanes[i].GetVertexModifierAs<CylinderBezierVertexModifier>().HasChanged = true;
            }
        }
    }


    private void OnValidate()
    {
        forceUpdate = true;
    }
}

[CustomEditor(typeof(CylinderBezierSegmentBehaviour)), CanEditMultipleObjects]
public class CylinderBezierEditorEditor : Editor
{
    protected virtual void OnSceneGUI()
    {
        CylinderBezierSegmentBehaviour cbb = (CylinderBezierSegmentBehaviour)target;
        var transform = (target as MonoBehaviour).transform;
        cbb.Segment = BezierSegmentBehaviour.DrawBezierHandles(cbb.Segment, transform);
    }
}