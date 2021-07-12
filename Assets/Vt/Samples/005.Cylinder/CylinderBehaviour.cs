using UnityEngine;

public class CylinderVertexModifier : VertexModifierBase
{
    public Matrix4x4 localToWorld;
    public float radius = 1f;
    public float sphereRatio = 1f;

    public override Vector3 Vertex(int x, int z)
    {
        float angle = Mathf.PI * 2 * x / (float)(VertexCount1D-1);
        return new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, z);
    }
}

public class CylinderBehaviour : MonoBehaviour
{
    [SerializeField] private string meshMaterialName = "Grid";
    [SerializeField] private float radius = 10f;
    [SerializeField] [Range(0, 7)] private int lod = 1;


    private ProcPlaneBehaviour[] procPlanes;
    private bool forceUpdate = false;

    private void Start()
    {
        procPlanes = new ProcPlaneBehaviour[1];
        VertexModifierBase vm = new CylinderVertexModifier();
        vm.Lod = lod;
        ProcPlaneCreateParameters createInfo = new ProcPlaneCreateParameters(
            name: "cylinder",
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
                procPlanes[i].GetVertexModifierAs<CylinderVertexModifier>().Lod = lod;
                procPlanes[i].GetVertexModifierAs<CylinderVertexModifier>().XSize = Mathf.Max(1, radius);
                procPlanes[i].GetVertexModifierAs<CylinderVertexModifier>().ZSize = Mathf.Max(1, radius);
                procPlanes[i].GetVertexModifierAs<CylinderVertexModifier>().radius = radius;
            }
        }
    }


    private void OnValidate()
    {
        forceUpdate = true;
    }
}
