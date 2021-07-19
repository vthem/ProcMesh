using UnityEngine;
using URandom = UnityEngine.Random;

public static class Random 
{
    public static Vector3 RandPointInBounds(Bounds bound)
    {
        Vector3 rnd = URandom.onUnitSphere;
        rnd.Scale(bound.size);
        return bound.min + rnd;
    }
}
