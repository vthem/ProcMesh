using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CylinderBezierVertexModifier : VertexModifierBase
{
    public Matrix4x4 localToWorld;
    public float radius = 1f;
    public float sphereRatio = 1f;
    public CylinderBezierProperties cylinderBezierProps;

    public override Vector3 Vertex(int x, int z)
    {
        float vCount = (float)(VertexCount1D - 1);
        float angle = Mathf.PI * 2 * x / vCount;
        float t = z / vCount;
        Vector3 pOnPath = Bezier.GetPoint(cylinderBezierProps.p0,
                                          cylinderBezierProps.p1,
                                          cylinderBezierProps.p2,
                                          cylinderBezierProps.p3,
                                          t);
        Vector3 tan = Bezier.GetFirstDerivative(cylinderBezierProps.p0,
                                        cylinderBezierProps.p1,
                                        cylinderBezierProps.p2,
                                        cylinderBezierProps.p3,
                                        t);
        Quaternion q = Quaternion.FromToRotation(Vector3.forward, tan);
        Vector3 pOnCircle = q * new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
        return pOnPath + pOnCircle;
    }
}

[System.Serializable]
public struct CylinderBezierProperties
{
    public Vector3 p0;
    public Vector3 p1;
    public Vector3 p2;
    public Vector3 p3;

    public CylinderBezierProperties(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        this.p0 = p0;
        this.p1 = p1;
        this.p2 = p2;
        this.p3 = p3;
    }
}

public class CylinderBezierBehaviour : MonoBehaviour
{
    [SerializeField] private string meshMaterialName = "Grid";
    [SerializeField] private float radius = 10f;
    [SerializeField] [Range(0, 7)] private int lod = 1;
    [SerializeField] public CylinderBezierProperties cylinderBezierProps = new CylinderBezierProperties(new Vector3(0, 0, -2),
                                                                                                        new Vector3(0, 0, -1),
                                                                                                        new Vector3(0, 0, 1),
                                                                                                        new Vector3(0, 0, 2));

    private ProcPlaneBehaviour[] procPlanes;
    private bool forceUpdate = false;

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
                procPlanes[i].GetVertexModifierAs<CylinderBezierVertexModifier>().cylinderBezierProps = cylinderBezierProps;
                procPlanes[i].GetVertexModifierAs<CylinderBezierVertexModifier>().HasChanged = true;
            }
        }
    }


    private void OnValidate()
    {
        forceUpdate = true;
    }
}

[CustomEditor(typeof(CylinderBezierBehaviour)), CanEditMultipleObjects]
public class CylinderBezierEditorEditor : Editor
{
    protected virtual void OnSceneGUI()
    {
        CylinderBezierBehaviour cbb = (CylinderBezierBehaviour)target;
        var transform = (target as MonoBehaviour).transform;

        DrawBezierPointHandle(ref cbb.cylinderBezierProps.p0, "p0");
        DrawBezierPointHandle(ref cbb.cylinderBezierProps.p1, "p1");
        DrawBezierPointHandle(ref cbb.cylinderBezierProps.p2, "p2");
        DrawBezierPointHandle(ref cbb.cylinderBezierProps.p3, "p3");

        Handles.color = Color.red;
        Handles.DrawLine(transform.TransformPoint(cbb.cylinderBezierProps.p0), transform.TransformPoint(cbb.cylinderBezierProps.p1));

        Handles.color = Color.green;
        Handles.DrawLine(transform.TransformPoint(cbb.cylinderBezierProps.p2), transform.TransformPoint(cbb.cylinderBezierProps.p3));

        Handles.color = Color.white;
        Vector3 b = Bezier.GetPoint(cbb.cylinderBezierProps.p0, cbb.cylinderBezierProps.p1, cbb.cylinderBezierProps.p2, cbb.cylinderBezierProps.p3, 0f);
        int count = 50;
        for (float t = 1; t <= count; t += 1)
        {
            Vector3 e = Bezier.GetPoint(cbb.cylinderBezierProps.p0, cbb.cylinderBezierProps.p1, cbb.cylinderBezierProps.p2, cbb.cylinderBezierProps.p3, t / (float)count);
            Handles.DrawLine(transform.TransformPoint(b), transform.TransformPoint(e));
            b = e;
        }
    }

    protected void DrawBezierPointHandle(ref Vector3 p, string name)
    {
        var transform = (target as MonoBehaviour).transform;
        EditorGUI.BeginChangeCheck();
        Quaternion q = Quaternion.identity;
        Vector3 newTargetPosition = Handles.PositionHandle(transform.TransformPoint(p), Quaternion.identity);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, "Change Look At Target Position");
            p = transform.InverseTransformPoint(newTargetPosition);
        }
        Handles.Label(transform.TransformPoint(p), name);
    }
}