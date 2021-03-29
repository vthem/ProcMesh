using UnityEngine;

public class Main : MonoBehaviour
{
    ProcPlaneBehaviour[] procPlanes;
    
    private void Start()
    {
        GameObject world = new GameObject("World");

        int procPlaneCount = 10;
        float procPlaneZSize = 10f;
        int maxLod = 7;
        procPlanes = new ProcPlaneBehaviour[procPlaneCount];
        for (int i = 0; i < procPlaneCount; ++i)
        {
            int lod = maxLod;
            if (maxLod > 0)
                maxLod--;

            ProcPlaneCreateInfo createInfo;
            createInfo.mName = $"{i}:lod{lod}";
            createInfo.mSize = Vector2.one * procPlaneZSize;
            createInfo.mLod = lod;
            createInfo.oParent = world.transform;

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
