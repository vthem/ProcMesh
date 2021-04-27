using UnityEngine;

public static class LodDistance
{
    public static int GetLod(float distance)
    {
        for (int i = 0; i < lodDistances.Length; ++i)
            if (distance < lodDistances[i].maxDistance)
                return lodDistances[i].lod;
        return 0;
    }
    #region private
    private struct LD
    {
        public int lod;
        public float maxDistance;
    }

    private static LD[] lodDistances = {
        new LD { lod = 7, maxDistance = 20f },
        new LD { lod = 6, maxDistance = 30f },
        new LD { lod = 4, maxDistance = 50f },
        new LD { lod = 2, maxDistance = 70f }
    };
    #endregion // private
}

public class InfiniteLandBehaviour : MonoBehaviour
{
    [SerializeField]
    private string meshMaterialName = "Grid";

    [SerializeField]
    private float moveWorldSpeed = 1f;

    [SerializeField] protected Vector3 perlinOffset = Vector3.one;
    [SerializeField] protected float perlinScale = 1f;
    [SerializeField] protected float xSize = 10f;
    [SerializeField] protected float zSize = 10f;

    private ProcPlaneBehaviour[] procPlanes;
    private Transform worldTransform;
    private bool forceUpdate = false;

    private void Start()
    {
        GameObject world = new GameObject("World");
        worldTransform = world.transform;

        int procPlaneCount = 6;
        int maxLod = 7;

        ProcPlaneCreateParameters[] procPlaneCreateInfos = new ProcPlaneCreateParameters[procPlaneCount];
        for (int i = 0; i < procPlaneCount; ++i)
        {
            int lod = maxLod;
            if (maxLod > 0)
                maxLod--;

            var vm = ScriptableObject.CreateInstance<PerlinVertexModifierScriptableObject>();
            vm.Lod = lod;
            vm.PerlinOffset = perlinOffset;
            vm.PerlinScale = perlinScale;
            vm.XSize = xSize;
            vm.ZSize = zSize;
            vm.HasChanged = true;
            ProcPlaneCreateParameters createInfo = new ProcPlaneCreateParameters(
                name: $"{i}",
                materialName: meshMaterialName,
                vertexModifier: vm
            ); ;
            createInfo.parent = world.transform;

            procPlaneCreateInfos[i] = createInfo;
        }

        procPlanes = new ProcPlaneBehaviour[procPlaneCount];
        for (int i = 0; i < procPlaneCount; ++i)
        {
            int lod = maxLod;
            if (maxLod > 0)
                maxLod--;

            ProcPlaneCreateParameters createInfo = procPlaneCreateInfos[i];
            if (i < procPlaneCount - 1)
                createInfo.lodInfo.frontLod = procPlaneCreateInfos[i + 1].vertexModifier.Lod;
            if (i > 0)
                createInfo.lodInfo.backLod = procPlaneCreateInfos[i - 1].vertexModifier.Lod;

            var procPlane = ProcPlaneBehaviour.Create(createInfo);
            procPlane.transform.localPosition = new Vector3(0, 0, zSize * i);
            procPlane.GetVertexModifierAs<PerlinVertexModifierScriptableObject>().LocalPosition = procPlane.transform.localPosition;
            procPlanes[i] = procPlane;
        }
    }

    private void Update()
    {
        MoveWorld(procPlanes, worldTransform, zSize, moveWorldSpeed);
        UpdateProcPlane(procPlanes, xSize, zSize, perlinScale, perlinOffset, forceUpdate);
        forceUpdate = false;
    }

    private static void MoveWorld(ProcPlaneBehaviour[] procPlanes, Transform transform, float zSize, float speed)
    {
        transform.localPosition += new Vector3(0, 0, -1) * speed * Time.deltaTime;
        for (int i = 0; i < procPlanes.Length; ++i)
        {
            var procPlane = procPlanes[i];
            if (procPlane.transform.position.z < -2f * zSize)
            {
                var nextIdx = (i - 1).Modulo(procPlanes.Length);
                procPlane.transform.localPosition = procPlanes[nextIdx].transform.localPosition + new Vector3(0, 0, zSize);
                procPlane.GetVertexModifierAs<PerlinVertexModifierScriptableObject>().LocalPosition = procPlane.transform.localPosition;
            }
        }
        UpdateLods(procPlanes);
    }

    private static void UpdateProcPlane(ProcPlaneBehaviour[] procPlanes, float xSize, float zSize, float perlinScale, Vector3 perlinOffset, bool forceUpdate)
    {
        for (int i = 0; i < procPlanes.Length; ++i)
        {
            var procPlane = procPlanes[i];
            if (forceUpdate)
            {
                procPlane.GetVertexModifierAs<PerlinVertexModifierScriptableObject>().PerlinOffset = perlinOffset;
                procPlane.GetVertexModifierAs<PerlinVertexModifierScriptableObject>().PerlinScale = perlinScale;
                procPlane.GetVertexModifierAs<PerlinVertexModifierScriptableObject>().XSize = xSize;
                procPlane.GetVertexModifierAs<PerlinVertexModifierScriptableObject>().ZSize = zSize;
                procPlane.GetVertexModifierAs<PerlinVertexModifierScriptableObject>().HasChanged = true;
            }
        }
    }

    private static void UpdateLods(ProcPlaneBehaviour[] procPlanes)
    {
        for (int cur = 0; cur < procPlanes.Length; ++cur)
        {
            var curPlane = procPlanes[cur];
            curPlane.GetVertexModifierAs<PerlinVertexModifierScriptableObject>().Lod = LodDistance.GetLod(curPlane.transform.position.z);
        }
        for (int cur = 0; cur < procPlanes.Length; ++cur)
        {
            var prev = (cur - 1).Modulo(procPlanes.Length);
            var next = (cur + 1).Modulo(procPlanes.Length);
            var curPlane = procPlanes[cur];
            var prevPlane = procPlanes[prev];
            var nextPlane = procPlanes[next];
            curPlane.FrontLod = nextPlane.VertexModifier.Lod;
            curPlane.BackLod = prevPlane.VertexModifier.Lod;
        }
    }

    private void OnValidate()
    {
        forceUpdate = true;
    }
}
