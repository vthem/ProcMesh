using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
#endif


[System.Serializable]
public class RandomPointGenerator
{
    public float minZLength = 1f;
    public float maxZLength = 5f;
    public float minXYLength = 1f;
    public float maxXYLength = 5f;

    private Vector3 lastBasePoint = Vector3.zero;

    public Vector3 GetNextPoint()
    {
        Vector2 unitCircle = UnityEngine.Random.insideUnitCircle;
        lastBasePoint = Vector3.forward * UnityEngine.Random.Range(minZLength, maxZLength) + lastBasePoint;
        return lastBasePoint + new Vector3(unitCircle.x, unitCircle.y) * UnityEngine.Random.Range(minXYLength, maxXYLength);
    }

    public void Reset(int seed)
    {
        lastBasePoint = Vector3.zero;
        UnityEngine.Random.InitState(seed);
    }
}

[ExecuteInEditMode]
public class RandomBezierPathBehaviour : MonoBehaviour
{
    [SerializeField]
    private RandomPointGenerator pointGenerator = new RandomPointGenerator();

    [SerializeField]
    private int numberOfSegment = 0;

    [SerializeField]
    private int seed = 0;

    [SerializeField]
    private BezierPath bezierPath = new BezierPath();
    public BezierPath Path => bezierPath;

    public Vector3 GetNextPoint()
    {
        return pointGenerator.GetNextPoint();
    }

    private void Update()
    {
        if (numberOfSegment != bezierPath.Count)
        {
            bezierPath.Clear();
            pointGenerator.Reset(seed);

            while (bezierPath.Count < numberOfSegment)
            {
                BezierSegment segment = bezierPath.AddEnd();
                segment.p0 = GetNextPoint();
                segment.p1 = GetNextPoint();
                segment.p2 = GetNextPoint();
                segment.p3 = GetNextPoint();
                bezierPath.SetSegment(bezierPath.Count - 1, segment);
            }
        }
    }

    private void OnValidate()
    {
        bezierPath.Clear();
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(RandomBezierPathBehaviour)), CanEditMultipleObjects]
public class RandomBezierPathEditor : Editor
{   
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RandomBezierPathBehaviour pathBehaviour = (RandomBezierPathBehaviour)target;
        BezierPath path = pathBehaviour.Path;

        if (GUILayout.Button("add segment"))
        {
            BezierSegment segment = path.AddEnd();
            segment.p0 = pathBehaviour.GetNextPoint();
            segment.p1 = pathBehaviour.GetNextPoint();
            segment.p2 = pathBehaviour.GetNextPoint();
            segment.p3 = pathBehaviour.GetNextPoint();
            path.SetSegment(path.Count - 1, segment);
            SceneView.RepaintAll();
        }
        if (GUILayout.Button("clear"))
        {
            path.Clear();
            SceneView.RepaintAll();
        }
        
    }

    protected virtual void OnSceneGUI()
    {
        RandomBezierPathBehaviour pathBehaviour = (RandomBezierPathBehaviour)target;
        BezierPath path = pathBehaviour.Path;
        var transform = (target as MonoBehaviour).transform;
        for (int i = 0; i < path.Count; ++i)
        {
            var segment = path.GetSegment(i);
            BezierSegmentBehaviour.DrawSegment(segment);
        }
    }
}
#endif