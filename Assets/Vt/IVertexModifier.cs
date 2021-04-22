using UnityEngine;

public interface IVertexModifier
{
    int Lod { get; }
    int VertexCount1D { get; }
    int VertexCount2D { get; }
    int IndiceCount { get; }

    bool Initialize();
    Vector3 Vertex(int x, int z);
}