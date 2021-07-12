using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BezierSegmentBehaviour : MonoBehaviour
{
    public BezierSegment segment = new BezierSegment(new Vector3(0, 0, -2),
                                                     new Vector3(0, 0, -1),
                                                     new Vector3(0, 0, 1),
                                                     new Vector3(0, 0, 2));

#if UNITY_EDITOR
    public static bool DrawBezierPointHandle(ref Vector3 p, string name, Object targetObject)
    {
        EditorGUI.BeginChangeCheck();
        Quaternion q = Quaternion.identity;
        Vector3 newTargetPosition = Handles.PositionHandle(p, Quaternion.identity);
        bool updated = false;
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(targetObject, $"Change Bezier point {name}");
            p = newTargetPosition;
            updated = true;
        }
        Handles.Label(p, name);
        return updated;
    }

    public static bool DrawBezierHandles(ref BezierSegment segment, string nameSuffix, Object targetObject)
    {
        bool updated = false;
        
        updated |= DrawBezierPointHandle(ref segment.p0, $"{nameSuffix}.p0", targetObject);
        updated |= DrawBezierPointHandle(ref segment.p1, $"{nameSuffix}.p1", targetObject);
        updated |= DrawBezierPointHandle(ref segment.p2, $"{nameSuffix}.p2", targetObject);
        updated |= DrawBezierPointHandle(ref segment.p3, $"{nameSuffix}.p3", targetObject);

        Handles.color = Color.red;
        Handles.DrawLine(segment.p0, segment.p1);

        Handles.color = Color.green;
        Handles.DrawLine(segment.p2, segment.p3);

        DrawSegment(segment);
        return updated;
    }

    public static void DrawSegment(BezierSegment segment)
    {
        Handles.color = Color.white;
        Vector3 b = segment.GetPoint(0f);
        int count = 50;
        for (float t = 1; t <= count; t += 1)
        {
            Vector3 e = segment.GetPoint(t / (float)count);
            Handles.DrawLine(b, e);
            b = e;
        }
    }
#endif
}
