using UnityEngine;

public class Main : MonoBehaviour
{
    [SerializeField]
    private string meshMaterialName = "Grid";

    private ProcPlaneBehaviour[] procPlanes;
    
    private void Start()
    {
        GameObject world = new GameObject("World");

        int procPlaneCount = 10;
        float procPlaneZSize = 10f;
        int maxLod = 7;

        ProcPlaneCreateParameters[] procPlaneCreateInfos = new ProcPlaneCreateParameters[procPlaneCount];
        for (int i = 0; i < procPlaneCount; ++i)
        {
            int lod = maxLod;
            if (maxLod > 0)
                maxLod--;

            ProcPlaneCreateParameters createInfo = new ProcPlaneCreateParameters(
                name: $"{i}:lod{lod}",
                lod: lod,
                size: Vector2.one * procPlaneZSize,
                materialName: meshMaterialName
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
            procPlane.transform.localPosition = new Vector3(0, 0, procPlaneZSize * i);
            procPlanes[i] = procPlane;
        }
    }

    private void Update()
    {
        MoveProcPlane(procPlanes);
    }

    private static void MoveProcPlane(ProcPlaneBehaviour[] procPlanes)
    {
        
    }
}
