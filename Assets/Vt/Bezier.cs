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
}

public class BezierPath
{
	// add segment
	// remove segment(idx)
	// get segment(t)
	// get segment(idx)
	// set segment(idx, segment, align neightbor)

	private List<BezierSegment> segments = new List<BezierSegment>(24);
}