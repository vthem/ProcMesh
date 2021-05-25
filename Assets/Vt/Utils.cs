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

public static class UnityExt
{
    public static T SafeGetComponent<T>(this MonoBehaviour monoBehaviour) where T : Component
    {
        T comp = monoBehaviour.GetComponent<T>();
        if (!comp)
        {
            comp = monoBehaviour.gameObject.AddComponent<T>();
        }
        return comp;
    }
}