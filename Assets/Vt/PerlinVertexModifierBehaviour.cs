using UnityEngine;

public class PerlinVertexModifier : ProcPlaneBehaviourVertexModifier
{
    public void Update(Vector3 localPosition, Vector3 perlinOffset, float perlinScale)
    {
        if (this.localPosition == localPosition
            && Mathf.Approximately(this.perlinScale, perlinScale)
            && this.perlinOffset == perlinOffset)
            return;
        this.localPosition = localPosition;
        this.perlinOffset = perlinOffset;
        this.perlinScale = perlinScale;
        perlinMatrix = Matrix4x4.TRS(perlinOffset, Quaternion.identity, Vector3.one * this.perlinScale) * Matrix4x4.TRS(localPosition, Quaternion.identity, Vector3.one);
        HasChanged = true;
    }

    public override Vector3 Exec(float x, float z)
    {
        var v = perlinMatrix.MultiplyPoint(new Vector3(x, 0, z));
        return new Vector3(x, Mathf.PerlinNoise(v.x, v.z), z);
    }

    private Vector3 perlinOffset = Vector3.one;
    private float perlinScale = 1f;
    private Vector3 localPosition;
    private Matrix4x4 perlinMatrix = Matrix4x4.identity;

}

public class PerlinVertexModifierBehaviour : MonoBehaviour
{
    public float PerlinScale { get => perlinScale; set => perlinScale = value; }
    public Vector3 PerlinOffset { get => perlinOffset; set => perlinOffset = value; }

    #region private
    [SerializeField]
    private float perlinScale = 1f;

    [SerializeField]
    private Vector3 perlinOffset = Vector3.one;   

    private ProcPlaneBehaviour procPlane;
    private PerlinVertexModifier vertexModifier;

    private void Awake()
    {
        procPlane = GetComponent<ProcPlaneBehaviour>();
        if (!procPlane)
        {
            Debug.LogError($"{nameof(PerlinVertexModifierBehaviour)} on object {name} requires a {nameof(ProcPlaneBehaviour)}");
            enabled = false;
            return;
        }

        vertexModifier = new PerlinVertexModifier();
        procPlane.VertexModifier = vertexModifier;
    }

    void Update()
    {
        vertexModifier.Update(transform.localPosition, perlinOffset, perlinScale);
    }
    #endregion // private
}
