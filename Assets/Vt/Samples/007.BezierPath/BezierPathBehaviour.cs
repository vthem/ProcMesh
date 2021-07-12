using System    .Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class BezierPathBehaviour : MonoBehaviour
{
    [SerializeField]
    private BezierPath bezierPath = new BezierPath();
        
    public BezierPath Path => bezierPath;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var childs = GetComponentInChildren<BezierSegmentBehaviour>();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(BezierPathBehaviour)), CanEditMultipleObjects]
public class BezierPathEditor : Editor
{
    bool debugDrawHandle = true;

    public override void OnInspectorGUI()
    {
        BezierPathBehaviour pathBehaviour = (BezierPathBehaviour)target;
        BezierPath path = pathBehaviour.Path;

        if (GUILayout.Button("add begin"))
        {
            path.AddBegin();
            SceneView.RepaintAll();
        }
        if (GUILayout.Button("add end"))
        {
            path.AddEnd();
            SceneView.RepaintAll();
        }
        if (GUILayout.Button("clear"))
        {
            path.Clear  ();
            SceneView.RepaintAll();
        }
        debugDrawHandle = EditorGUILayout.Toggle("debug draw handle", debugDrawHandle);
        DrawDefaultInspector();
    }

    protected virtual void OnSceneGUI()
    {
        BezierPathBehaviour pathBehaviour = (BezierPathBehaviour)target;
        BezierPath path = pathBehaviour.Path;
        var transform = (target as MonoBehaviour).transform;
        for (int i = 0; i < path.Count; ++i)
        {
            var segment = path.GetSegment(i);
            if (debugDrawHandle)
            {
                if (BezierSegmentBehaviour.DrawBezierHandles(ref segment, $"{i}", target))
                {
                    path.SetSegment(i, segment);
                }
            }
            else
            {
                BezierSegmentBehaviour.DrawSegment(segment);
            }
        }
    }
}
#endif