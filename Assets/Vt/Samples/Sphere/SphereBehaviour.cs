using UnityEngine;

public class SphereVertexModifier : VertexModifierBase
{
    public Matrix4x4 localToWorld;
    public float radius = 1f;
    public float sphereRatio = 1f;

    public override Vector3 Vertex(int x, int z)
    {
        Vector3 v = base.Vertex(x, z);
        v.y = radius * .5f;
        return Vector3.Lerp(v, v.normalized * radius, sphereRatio);
    }
}

public class SphereBehaviour : MonoBehaviour
{
    [SerializeField] private string meshMaterialName = "Grid";
    [SerializeField] private float radius = 10f;
    [SerializeField] [Range(0, 7)] private int lod = 1;
    [SerializeField] [Range(0, 1f)] private float sphereRatio = 1f;


    private ProcPlaneBehaviour[] procPlanes;
    private bool forceUpdate = false;

    private void Start()
    {
        procPlanes = new ProcPlaneBehaviour[6];
        var infos = new (Vector3 dir, string dirName, int idx)[] {
            (Vector3.up,      "up",      0),
            (Vector3.down,    "down",    1),
            (Vector3.left,    "left",    2),
            (Vector3.right,   "right",   3),
            (Vector3.forward, "forward", 4),
            (Vector3.back,    "back",    5)
        };
        foreach (var info in infos)
            procPlanes[info.idx] = CreateProcPlane(info.dir, info.dirName);
    }

    private ProcPlaneBehaviour CreateProcPlane(Vector3 dir, string dirName)
    {
        VertexModifierBase vm = new SphereVertexModifier();
        vm.Lod = lod;
        ProcPlaneCreateParameters createInfo = new ProcPlaneCreateParameters(
            name: dirName,
            materialName: meshMaterialName,
            vertexModifier: vm
        );
        var procPlane = ProcPlaneBehaviour.Create(createInfo);
        //procPlane.transform.localPosition = dir * size * .5f;
        procPlane.transform.up = dir;
        procPlane.GetVertexModifierAs<SphereVertexModifier>().localToWorld = procPlane.transform.localToWorldMatrix;
        return procPlane;
    }

    private void Update()
    {
        if (forceUpdate)
        {
            for (int i = 0; i < procPlanes.Length; ++i)
            {
                procPlanes[i].GetVertexModifierAs<SphereVertexModifier>().Lod = lod;
                procPlanes[i].GetVertexModifierAs<SphereVertexModifier>().XSize = Mathf.Max(1, radius);
                procPlanes[i].GetVertexModifierAs<SphereVertexModifier>().ZSize = Mathf.Max(1, radius);
                procPlanes[i].GetVertexModifierAs<SphereVertexModifier>().radius = radius;
                procPlanes[i].GetVertexModifierAs<SphereVertexModifier>().sphereRatio = sphereRatio;
            }
        }
    }


    private void OnValidate()
    {
        forceUpdate = true;
    }
}
