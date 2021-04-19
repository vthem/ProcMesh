using UnityEngine;

public static class Utils
{
    public static int Modulo(this int n, int range)
    {
        return (n % range + range) % range;
    }

    public static (bool, int) SetValue(this int target, int v)
    {
        return (target != v, v);
    }

    public static (bool, float) SetValue(this float target, float v)
    {
        return (!Mathf.Approximately(target, v), v);
    }

    public static (bool, Vector3) SetValue(this Vector3 target, Vector3 v)
    {
        return (target != v, v);
    }

    public static float Remap(this float value, float fromBegin, float fromEnd, float toBegin, float toEnd)
    {
        return (value - fromBegin) / (fromEnd - fromBegin) * (toEnd - toBegin) + toBegin;
    }
}

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

    [SerializeField]
    private float procPlaneSizeZ = 10f;

    [SerializeField]
    private float perlinScale = 1f;

    [SerializeField]
    private Vector3 perlinOffset = Vector3.one;

    private ProcPlaneBehaviour[] procPlanes;
    private Transform worldTransform;

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

            ProcPlaneCreateParameters createInfo = new ProcPlaneCreateParameters(
                name: $"{i}",
                lod: lod,
                materialName: meshMaterialName,
                null /* vertexModifier: new PerlinVertexModifier(procPlaneSizeZ, procPlaneSizeZ) */
            );
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
                createInfo.meshInfo.frontLod = procPlaneCreateInfos[i + 1].meshInfo.lod;
            if (i > 0)
                createInfo.meshInfo.backLod = procPlaneCreateInfos[i - 1].meshInfo.lod;

            var procPlane = ProcPlaneBehaviour.Create(createInfo);
            procPlane.transform.localPosition = new Vector3(0, 0, procPlaneSizeZ * i);
            procPlanes[i] = procPlane;
        }
    }

    private void Update()
    {
        MoveWorld(worldTransform, moveWorldSpeed);
        UpdateProcPlane(procPlanes, procPlaneSizeZ, perlinScale, perlinOffset);
    }

    private static void MoveWorld(Transform transform, float speed)
    {
        transform.localPosition += new Vector3(0, 0, -1) * speed * Time.deltaTime;
    }

    private static void UpdateProcPlane(ProcPlaneBehaviour[] procPlanes, float procPlaneSizeZ, float perlinScale, Vector3 perlinOffset)
    {
        for (int i = 0; i < procPlanes.Length; ++i)
        {
            var procPlane = procPlanes[i];
            if (procPlane.transform.position.z < -2f * procPlaneSizeZ)
            {
                var nextIdx = (i - 1).Modulo(procPlanes.Length);
                procPlane.transform.localPosition = procPlanes[nextIdx].transform.localPosition + new Vector3(0, 0, procPlaneSizeZ);
                //var vModifier = (procPlane.VertexModifier as PerlinVertexModifier);
                //vModifier.PerlinOffset = perlinOffset;
                //vModifier.LocalPosition = procPlane.transform.localPosition;
                //vModifier.PerlinScale = perlinScale;
                //vModifier.Update();
            }
        }
        UpdateLods(procPlanes);
    }

    //public Vector3 VertexFunc(int x, int z)
    //{
    //    float xVal = xStart + x * xDelta;
    //    float zVal = zStart + z * zDelta;
    //    var v = perlinMatrix.MultiplyPoint(new Vector3(xVal, 0, zVal));
    //    return new Vector3(xVal, Mathf.PerlinNoise(v.x, v.z), zVal);
    //}

    private static void UpdateLods(ProcPlaneBehaviour[] procPlanes)
    {
        for (int cur = 0; cur < procPlanes.Length; ++cur)
        {
            var curPlane = procPlanes[cur];
            curPlane.Lod = LodDistance.GetLod(curPlane.transform.position.z);
        }
        for (int cur = 0; cur < procPlanes.Length; ++cur)
        {
            var prev = (cur-1).Modulo(procPlanes.Length);
            var next = (cur+1).Modulo(procPlanes.Length);
            var curPlane = procPlanes[cur];
            var prevPlane = procPlanes[prev];
            var nextPlane = procPlanes[next];
            curPlane.FrontLod = nextPlane.Lod;
            curPlane.BackLod = prevPlane.Lod;
        }
    }
}
