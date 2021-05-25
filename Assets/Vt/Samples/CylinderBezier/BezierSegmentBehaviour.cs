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
    public static void DrawBezierPointHandle(ref Vector3 p, string name, Transform transform)
    {
        EditorGUI.BeginChangeCheck();
        Quaternion q = Quaternion.identity;
        Vector3 newTargetPosition = Handles.PositionHandle(transform.TransformPoint(p), Quaternion.identity);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(transform, $"Change Bezier point {name}");
            p = transform.InverseTransformPoint(newTargetPosition);
        }
        Handles.Label(transform.TransformPoint(p), name);
    }

    public static BezierSegment DrawBezierHandles(BezierSegment segment, Transform transform)
    {
        DrawBezierPointHandle(ref segment.p0, "p0", transform);
        DrawBezierPointHandle(ref segment.p1, "p1", transform);
        DrawBezierPointHandle(ref segment.p2, "p2", transform);
        DrawBezierPointHandle(ref segment.p3, "p3", transform);

        Handles.color = Color.red;
        Handles.DrawLine(transform.TransformPoint(segment.p0), transform.TransformPoint(segment.p1));

        Handles.color = Color.green;
        Handles.DrawLine(transform.TransformPoint(segment.p2), transform.TransformPoint(segment.p3));

        Handles.color = Color.white;
        Vector3 b = segment.GetPoint(0f);
        int count = 50;
        for (float t = 1; t <= count; t += 1)
        {
            Vector3 e = segment.GetPoint(t / (float)count);
            Handles.DrawLine(transform.TransformPoint(b), transform.TransformPoint(e));
            b = e;
        }
        return segment;
    }
#endif
}
