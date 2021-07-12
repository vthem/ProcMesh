using System;
using System.Collections.Generic;
using UnityEngine;
// from https://catlikecoding.com/unity/tutorials/curves-and-splines/

public static class BezierMath {

	public static Vector3 GetPoint (Vector3 p0, Vector3 p1, Vector3 p2, float t) {
		t = Mathf.Clamp01(t);
		float oneMinusT = 1f - t;
		return
			oneMinusT * oneMinusT * p0 +
			2f * oneMinusT * t * p1 +
			t * t * p2;
	}

	public static Vector3 GetFirstDerivative (Vector3 p0, Vector3 p1, Vector3 p2, float t) {
		return
			2f * (1f - t) * (p1 - p0) +
			2f * t * (p2 - p1);
	}

	public static Vector3 GetPoint (Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
		t = Mathf.Clamp01(t);
		float OneMinusT = 1f - t;
		return
			OneMinusT * OneMinusT * OneMinusT * p0 +
			3f * OneMinusT * OneMinusT * t * p1 +
			3f * OneMinusT * t * t * p2 +
			t * t * t * p3;
	}

	public static Vector3 GetFirstDerivative (Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
		t = Mathf.Clamp01(t);
		float oneMinusT = 1f - t;
		return
			3f * oneMinusT * oneMinusT * (p1 - p0) +
			6f * oneMinusT * t * (p2 - p1) +
			3f * t * t * (p3 - p2);
	}
}

[System.Serializable]
public struct BezierSegment
{
	public Vector3 p0;
	public Vector3 p1;
	public Vector3 p2;
	public Vector3 p3;

	public BezierSegment(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
	{
		this.p0 = p0;
		this.p1 = p1;
		this.p2 = p2;
		this.p3 = p3;
	}

	public Vector3 GetPoint(float t)
	{
		return BezierMath.GetPoint(p0, p1, p2, p3, t);
	}

	public Vector3 GetFirstDerivative(float t)
	{
		return BezierMath.GetFirstDerivative(p0, p1, p2, p3, t);
	}

    public void JoinBegin(BezierSegment end)
    {
        p0 = end.p3;
        var vec = end.p3 - end.p2;
        p1 = p0 + vec;
    }

    public void JoinEnd(BezierSegment begin)
    {
        p3 = begin.p0;
        var vec = begin.p0 - begin.p1;
        p2 = p3 + vec;
    }

    public static BezierSegment Default => new BezierSegment(new Vector3(0, 0, -2),
															 new Vector3(0, 0, -1),
															 new Vector3(0, 0, 1),
															 new Vector3(0, 0, 2));

    public BezierSegment Transform(Matrix4x4 mat)
    {
        return new BezierSegment(mat.MultiplyPoint3x4(p0),
            mat.MultiplyPoint3x4(p1),
            mat.MultiplyPoint3x4(p2),
            mat.MultiplyPoint3x4(p3));
    }
}

[System.Serializable]
public class BezierPath
{
	// add segment
	// remove segment(idx)
	// get segment(t)
	// get segment(idx)
	// set segment(idx, segment, align neightbor)

	public List<BezierSegment> segments = new List<BezierSegment>(24);

    public int Count => segments.Count;

    public BezierSegment GetSegment(int index)
    {
        return segments[index];
    }

    public void Clear()
    {
        segments.Clear();
    }

    public void SetSegment(int index, BezierSegment segment)
    {
       
        if (index + 1 < Count)
        {
            var next = segments[index + 1];
            next.JoinBegin(segment);
            segments[index + 1] = next;
        }
        if (index - 1 >= 0)
        {
            var previous = segments[index - 1];
            previous.JoinEnd(segment);
            segments[index - 1] = previous;
        }
        segments[index] = segment;
    }

	public BezierSegment AddSegment()
	{
		BezierSegment segment = BezierSegment.Default;
		if (segments.Count > 0)
		{
			segment.p1 = segments[segments.Count - 1].p2;
			segment.p0 = -segments[segments.Count - 1].p3;
		}
		segments.Add(segment);
		return segment;
	}

    public BezierSegment AddBegin()
    {
        BezierSegment segment = BezierSegment.Default;
        if (Count > 0)
        {
            segment.JoinEnd(segments[0]);
        }
        segment.p1 = segment.p2 - Vector3.forward;
        segment.p0 = segment.p1 - Vector3.forward;
        segments.Insert(0, segment);
        return segment;
    }

    public BezierSegment AddEnd()
    {
        if (Count == 0)
            return AddBegin();
            
        BezierSegment segment = BezierSegment.Default;
        segment.JoinBegin(segments[Count - 1]);
        segment.p2 = segment.p1 + Vector3.forward;
        segment.p3 = segment.p2 + Vector3.forward;
        segments.Insert(Count - 1, segment);
        return segment;
    }

    public Vector3 GetPoint(float t)
	{
        float f = t * segments.Count;
        int idx = Mathf.FloorToInt(f);
        float r = f - idx;
		return segments[idx].GetPoint(r);
	}

    public Vector3 GetFirstDerivative(float t)
    {
        float f = t * segments.Count;
        int idx = Mathf.FloorToInt(f);
        float r = f - idx;
        return segments[idx].GetFirstDerivative(r);
    }
}