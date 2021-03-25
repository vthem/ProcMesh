using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class ProcQuad : MonoBehaviour
{
    public int lod = 0;
    public Vector2 size = Vector2.one;
    public float resolution = 20f;
    public Vector2Int VertexCount => new Vector2Int(Mathf.RoundToInt(size.x * resolution), Mathf.RoundToInt(size.y * resolution));

    private MeshInfo meshInfo;

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    private struct ExampleVertex
    {
        public Vector3 pos;
    }

    // Start is called before the first frame update
    //void Start()
    //{
    //    var mesh = GetComponent<MeshFilter>().sharedMesh;
    //    if (!mesh)
    //    {
    //        mesh = new Mesh();
    //        GetComponent<MeshFilter>().sharedMesh = mesh;
    //    }
    //    mesh.MarkDynamic();

    //    GenPlane5(mesh, Mathf.RoundToInt(size.x * resolution), Mathf.RoundToInt(size.x * resolution), size.x, size.y, Perlin);
    //}

    private void Update()
    {
        if (!IsMeshInfoValid())
        {
            transform.hasChanged = false;
            ReleaseMeshInfo();
            AllocateMeshInfo();
            GenPlane6(meshInfo, Perlin);
        }
    }

    void AllocateMeshInfo()
    {
        var mesh = GetComponent<MeshFilter>().sharedMesh;
        if (!mesh)
        {
            mesh = new Mesh();
            GetComponent<MeshFilter>().sharedMesh = mesh;
        }
        mesh.MarkDynamic();
        meshInfo.mesh = mesh;
        meshInfo.size = size;
        meshInfo.vertexCount = VertexCount;

        meshInfo.vertices = new NativeArray<ExampleVertex>(meshInfo.VertexCount, Allocator.Persistent);
        meshInfo.indices = new NativeArray<ushort>(meshInfo.IndiceCount, Allocator.Persistent);
    }

    void ReleaseMeshInfo()
    {
        if (meshInfo.vertices.IsCreated)
        {
            meshInfo.vertices.Dispose();
        }
        if (meshInfo.indices.IsCreated)
        {
            meshInfo.indices.Dispose();
        }
    }

    bool IsMeshInfoValid()
    {
        if (transform.hasChanged)
            return false;
        if (!meshInfo.mesh || !meshInfo.indices.IsCreated || !meshInfo.vertices.IsCreated)
            return false;
        return meshInfo.size == size && meshInfo.vertexCount == VertexCount;
    }

    float Perlin(float x, float z)
    {
        var v = transform.TransformPoint(new Vector3(x, 0, z));
        return Mathf.PerlinNoise(v.x, v.z);
    }

    static void GenObj(GameObject template, string name, int lod)
    {
        var obj = GameObject.Instantiate(template);
        obj.name = name;
        obj.transform.parent = template.transform;

        var mesh = obj.GetComponent<MeshFilter>().sharedMesh;
        if (!mesh)
        {
            mesh = new Mesh();
            obj.GetComponent<MeshFilter>().sharedMesh = mesh;
        }
        mesh.MarkDynamic();

        //if (lod == 0)
        //{
        //    GenPlane5(mesh, 40, 20, 2f, 1f);
        //}
        //else if (lod == 1)
        //{
        //    GenPlane5(mesh, 20, 10, 2f, 1f);
        //}
        //else if (lod == 2)
        //{
        //    GenPlane5(mesh, 10, 5, 2f, 1f);
        //}
    }

    static void GenPlane(Mesh mesh)
    {
        // specify vertex count and layout
        var layout = new[]
        {
            new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3)
        };
        var vertexCount = 4;
        mesh.SetVertexBufferParams(vertexCount, layout);

        // set vertex data
        var verts = new NativeArray<ExampleVertex>(vertexCount, Allocator.Temp);

        verts[0] = new ExampleVertex { pos = new Vector3(-.5f, 0, -.5f) };
        verts[1] = new ExampleVertex { pos = new Vector3(.5f, 0, -.5f) };
        verts[2] = new ExampleVertex { pos = new Vector3(-.5f, 0, .5f) };
        verts[3] = new ExampleVertex { pos = new Vector3(.5f, 0, .5f) };

        mesh.SetVertexBufferData(verts, 0, 0, vertexCount);
        mesh.SetIndexBufferParams(6, IndexFormat.UInt16);

        var indices = new NativeArray<System.UInt16>(6, Allocator.Temp);
        indices[0] = 0;
        indices[1] = 2;
        indices[2] = 1;
        indices[3] = 2;
        indices[4] = 3;
        indices[5] = 1;

        mesh.SetIndexBufferData(indices, 0, 0, 6);
        mesh.subMeshCount = 1;
        mesh.SetSubMesh(0, new SubMeshDescriptor(0, 6, MeshTopology.Triangles));
    }
    static void GenPlane2(Mesh mesh, int xCount, int zCount)
    {
        // specify vertex count and layout
        var layout = new[]
        {
            new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3)
        };
        var vertexCount = xCount * zCount;
        mesh.SetVertexBufferParams(vertexCount, layout);

        // set vertex data
        var verts = new NativeArray<ExampleVertex>(vertexCount, Allocator.Temp);
        var xDelta = 1f / (float)(xCount - 1);
        var zDelta = 1f / (float)(zCount - 1);

        for (int i = 0, z = 0; z < zCount; z++)
        {
            for (int x = 0; x < xCount; x++, i++)
            {
                verts[i] = new ExampleVertex { pos = new Vector3(-.5f + x * xDelta, 0, -.5f + z * zDelta) };
            }
        }

        mesh.SetVertexBufferData(verts, 0, 0, vertexCount);

        var triangleCount = (xCount - 1) * (zCount - 1) * 2;
        var indiceCount = triangleCount * 3;
        mesh.SetIndexBufferParams(indiceCount, IndexFormat.UInt16);
        var indices = new NativeArray<ushort>(indiceCount, Allocator.Temp);

        int idx = 0;
        for (int z = 0; z < zCount - 1; ++z)
        {
            for (int x = 0; x < xCount - 1; ++x)
            {
                var vi = z * (xCount) + x;
                ushort p1 = (ushort)vi;
                ushort p2 = (ushort)(vi + 1);
                ushort p3 = (ushort)(vi + xCount);
                ushort p4 = (ushort)(vi + xCount + 1);
                indices[idx++] = p1;
                indices[idx++] = p3;
                indices[idx++] = p2;
                indices[idx++] = p2;
                indices[idx++] = p3;
                indices[idx++] = p4;
            }
        }

        mesh.SetIndexBufferData(indices, 0, 0, indiceCount);
        mesh.subMeshCount = 1;
        mesh.SetSubMesh(0, new SubMeshDescriptor(0, indiceCount, MeshTopology.Triangles));
    }
    static void GenPlane3(Mesh mesh, int xCount, int zCount, float xSize, float zSize)
    {
        // specify vertex count and layout
        var layout = new[]
        {
            new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3)
        };
        var vertexCount = xCount * zCount;
        mesh.SetVertexBufferParams(vertexCount, layout);

        // set vertex data
        var verts = new NativeArray<ExampleVertex>(vertexCount, Allocator.Temp);
        var xDelta = xSize / (float)(xCount - 1);
        var zDelta = zSize / (float)(zCount - 1);

        var xStart = -xSize * 0.5f;
        var zStart = -zSize * 0.5f;

        for (int i = 0, z = 0; z < zCount; z++)
        {
            for (int x = 0; x < xCount; x++, i++)
            {
                verts[i] = new ExampleVertex { pos = new Vector3(xStart + x * xDelta, 0, zStart + z * zDelta) };
            }
        }

        mesh.SetVertexBufferData(verts, 0, 0, vertexCount);

        var triangleCount = (xCount - 1) * (zCount - 1) * 2;
        var indiceCount = triangleCount * 3;
        mesh.SetIndexBufferParams(indiceCount, IndexFormat.UInt16);
        var indices = new NativeArray<ushort>(indiceCount, Allocator.Temp);

        int idx = 0;
        for (int z = 0; z < zCount - 1; ++z)
        {
            for (int x = 0; x < xCount - 1; ++x)
            {
                var vi = z * (xCount) + x;
                ushort p1 = (ushort)vi;
                ushort p2 = (ushort)(vi + 1);
                ushort p3 = (ushort)(vi + xCount);
                ushort p4 = (ushort)(vi + xCount + 1);
                indices[idx++] = p1;
                indices[idx++] = p3;
                indices[idx++] = p2;
                indices[idx++] = p2;
                indices[idx++] = p3;
                indices[idx++] = p4;
            }
        }

        mesh.SetIndexBufferData(indices, 0, 0, indiceCount);
        mesh.subMeshCount = 1;
        mesh.SetSubMesh(0, new SubMeshDescriptor(0, indiceCount, MeshTopology.Triangles));
    }
    static void GenPlane4(Mesh mesh, int xCount, int zCount, float xSize, float zSize)
    {
        // specify vertex count and layout
        var layout = new[]
        {
            new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3)
        };
        var vertexCount = xCount * zCount;
        mesh.SetVertexBufferParams(vertexCount, layout);

        // set vertex data
        var verts = new NativeArray<ExampleVertex>(vertexCount, Allocator.Temp);
        var xDelta = xSize / (float)(xCount - 1);
        var zDelta = zSize / (float)(zCount - 1);

        var xStart = -xSize * 0.5f;
        var zStart = -zSize * 0.5f;

        for (int i = 0, z = 0; z < zCount; z++)
        {
            for (int x = 0; x < xCount; x++, i++)
            {
                var xPos = xStart + x * xDelta;
                var zPos = zStart + z * zDelta;
                var yPos = Mathf.PerlinNoise(xPos, zPos);
                verts[i] = new ExampleVertex { pos = new Vector3(xPos, yPos, zPos) };
            }
        }

        mesh.SetVertexBufferData(verts, 0, 0, vertexCount);

        var triangleCount = (xCount - 1) * (zCount - 1) * 2;
        var indiceCount = triangleCount * 3;
        mesh.SetIndexBufferParams(indiceCount, IndexFormat.UInt16);
        var indices = new NativeArray<ushort>(indiceCount, Allocator.Temp);

        int idx = 0;
        for (int z = 0; z < zCount - 1; ++z)
        {
            for (int x = 0; x < xCount - 1; ++x)
            {
                var vi = z * (xCount) + x;
                ushort p1 = (ushort)vi;
                ushort p2 = (ushort)(vi + 1);
                ushort p3 = (ushort)(vi + xCount);
                ushort p4 = (ushort)(vi + xCount + 1);
                indices[idx++] = p1;
                indices[idx++] = p3;
                indices[idx++] = p2;
                indices[idx++] = p2;
                indices[idx++] = p3;
                indices[idx++] = p4;
            }
        }

        mesh.SetIndexBufferData(indices, 0, 0, indiceCount);
        mesh.subMeshCount = 1;
        mesh.SetSubMesh(0, new SubMeshDescriptor(0, indiceCount, MeshTopology.Triangles));
    }
    static void GenPlane5(Mesh mesh, int xCount, int zCount, float xSize, float zSize, Func<float, float, float> height)
    {
        // specify vertex count and layout
        var layout = new[]
        {
            new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3)
        };
        var vertexCount = xCount * zCount;
        mesh.SetVertexBufferParams(vertexCount, layout);

        // set vertex data
        var verts = new NativeArray<ExampleVertex>(vertexCount, Allocator.Temp);
        var xDelta = xSize / (float)(xCount - 1);
        var zDelta = zSize / (float)(zCount - 1);

        var xStart = -xSize * 0.5f;
        var zStart = -zSize * 0.5f;

        // seams first
        // left
        int x = 0;
        int z = 0;
        for (z = 0; z < zCount; z++)
        {
            var i = x + z * xCount;
            var xPos = xStart + x * xDelta;
            var zPos = zStart + z * zDelta;
            var yPos = height(xPos, zPos);
            verts[i] = new ExampleVertex { pos = new Vector3(xPos, yPos, zPos) };
        }
        // up
        z = zCount - 1;
        for (x = 0; x < xCount; x++)
        {
            var i = x + z * xCount;
            var xPos = xStart + x * xDelta;
            var zPos = zStart + z * zDelta;
            var yPos = Mathf.PerlinNoise(xPos, zPos);
            verts[i] = new ExampleVertex { pos = new Vector3(xPos, yPos, zPos) };
        }
        // right
        x = xCount - 1;
        for (z = 0; z < zCount; z++)
        {
            var i = x + z * xCount;
            var xPos = xStart + x * xDelta;
            var zPos = zStart + z * zDelta;
            var yPos = Mathf.PerlinNoise(xPos, zPos);
            verts[i] = new ExampleVertex { pos = new Vector3(xPos, yPos, zPos) };
        }
        // bottom
        z = 0;
        for (x = 0; x < xCount; x++)
        {
            var i = x + z * xCount;
            var xPos = xStart + x * xDelta;
            var zPos = zStart + z * zDelta;
            var yPos = Mathf.PerlinNoise(xPos, zPos);
            verts[i] = new ExampleVertex { pos = new Vector3(xPos, yPos, zPos) };
        }

        // center
        for (z = 1; z < zCount-1; z++)
        {
            for (x = 1; x < xCount-1; x++)
            {
                var i = x + z * xCount;
                var xPos = xStart + x * xDelta;
                var zPos = zStart + z * zDelta;
                var yPos = Mathf.PerlinNoise(xPos, zPos);
                verts[i] = new ExampleVertex { pos = new Vector3(xPos, yPos, zPos) };
            }
        }

        mesh.SetVertexBufferData(verts, 0, 0, vertexCount);

        var triangleCount = (xCount - 1) * (zCount - 1) * 2;
        var indiceCount = triangleCount * 3;
        mesh.SetIndexBufferParams(indiceCount, IndexFormat.UInt16);
        var indices = new NativeArray<ushort>(indiceCount, Allocator.Temp);

        int idx = 0;
        for (z = 0; z < zCount - 1; ++z)
        {
            for (x = 0; x < xCount - 1; ++x)
            {
                var vi = z * (xCount) + x;
                ushort p1 = (ushort)vi;
                ushort p2 = (ushort)(vi + 1);
                ushort p3 = (ushort)(vi + xCount);
                ushort p4 = (ushort)(vi + xCount + 1);
                indices[idx++] = p1;
                indices[idx++] = p3;
                indices[idx++] = p2;
                indices[idx++] = p2;
                indices[idx++] = p3;
                indices[idx++] = p4;
            }
        }

        mesh.SetIndexBufferData(indices, 0, 0, indiceCount);
        mesh.subMeshCount = 1;
        mesh.SetSubMesh(0, new SubMeshDescriptor(0, indiceCount, MeshTopology.Triangles));
    }

    struct MeshInfo
    {
        public Mesh mesh;
        public NativeArray<ExampleVertex> vertices;
        public NativeArray<ushort> indices;
        public Vector2Int vertexCount;
        public Vector2 size;
        public int VertexCount => vertexCount.x * vertexCount.y;
        public int IndiceCount => (vertexCount.x - 1) * (vertexCount.y - 1) * 2 * 3;
    }

    static void GenPlane6(MeshInfo info, Func<float, float, float> height)
    {
        int xCount = info.vertexCount.x;
        int zCount = info.vertexCount.y;
        float xSize = info.size.x;
        float zSize = info.size.y;
        Mesh mesh = info.mesh;
        var verts = info.vertices; // new NativeArray<ExampleVertex>(vertexCount, Allocator.Temp);
        var indices = info.indices; // new NativeArray<ushort>(indiceCount, Allocator.Temp);
        var vertexCount = info.VertexCount;
        var indiceCount = info.IndiceCount;

        // specify vertex count and layout
        var layout = new[]
        {
            new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3)
        };
        mesh.SetVertexBufferParams(vertexCount, layout);

        // set vertex data
        
        var xDelta = xSize / (float)(xCount - 1);
        var zDelta = zSize / (float)(zCount - 1);

        var xStart = -xSize * 0.5f;
        var zStart = -zSize * 0.5f;

        // seams first
        // left
        int x = 0;
        int z = 0;
        for (z = 0; z < zCount; z++)
        {
            var i = x + z * xCount;
            var xPos = xStart + x * xDelta;
            var zPos = zStart + z * zDelta;
            var yPos = height(xPos, zPos);
            verts[i] = new ExampleVertex { pos = new Vector3(xPos, yPos, zPos) };
        }
        // up
        z = zCount - 1;
        for (x = 0; x < xCount; x++)
        {
            var i = x + z * xCount;
            var xPos = xStart + x * xDelta;
            var zPos = zStart + z * zDelta;
            var yPos = height(xPos, zPos);
            verts[i] = new ExampleVertex { pos = new Vector3(xPos, yPos, zPos) };
        }
        // right
        x = xCount - 1;
        for (z = 0; z < zCount; z++)
        {
            var i = x + z * xCount;
            var xPos = xStart + x * xDelta;
            var zPos = zStart + z * zDelta;
            var yPos = height(xPos, zPos);
            verts[i] = new ExampleVertex { pos = new Vector3(xPos, yPos, zPos) };
        }
        // bottom
        z = 0;
        for (x = 0; x < xCount; x++)
        {
            var i = x + z * xCount;
            var xPos = xStart + x * xDelta;
            var zPos = zStart + z * zDelta;
            var yPos = height(xPos, zPos);
            verts[i] = new ExampleVertex { pos = new Vector3(xPos, yPos, zPos) };
        }

        // center
        for (z = 1; z < zCount - 1; z++)
        {
            for (x = 1; x < xCount - 1; x++)
            {
                var i = x + z * xCount;
                var xPos = xStart + x * xDelta;
                var zPos = zStart + z * zDelta;
                var yPos = height(xPos, zPos);
                verts[i] = new ExampleVertex { pos = new Vector3(xPos, yPos, zPos) };
            }
        }

        mesh.SetVertexBufferData(verts, 0, 0, vertexCount);       
        mesh.SetIndexBufferParams(indiceCount, IndexFormat.UInt16);        

        int idx = 0;
        for (z = 0; z < zCount - 1; ++z)
        {
            for (x = 0; x < xCount - 1; ++x)
            {
                var vi = z * (xCount) + x;
                ushort p1 = (ushort)vi;
                ushort p2 = (ushort)(vi + 1);
                ushort p3 = (ushort)(vi + xCount);
                ushort p4 = (ushort)(vi + xCount + 1);
                indices[idx++] = p1;
                indices[idx++] = p3;
                indices[idx++] = p2;
                indices[idx++] = p2;
                indices[idx++] = p3;
                indices[idx++] = p4;
            }
        }

        mesh.SetIndexBufferData(indices, 0, 0, indiceCount);
        mesh.subMeshCount = 1;
        mesh.SetSubMesh(0, new SubMeshDescriptor(0, indiceCount, MeshTopology.Triangles));
    }
}
