
using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
struct ExampleVertex
{
    public Vector3 pos;
}

struct MeshInfo
{
    public Mesh mesh;
    public NativeArray<ExampleVertex> vertices;
    public NativeArray<ushort> indices;
    public int lod;
    public Vector2 size;
    public int VertexCount1D => (int)Mathf.Pow(2, lod) + 1;
    public int VertexCount2D
    {
        get
        {
            var vc1d = VertexCount1D;
            return vc1d * vc1d;
        }
    }
    public int IndiceCount
    {
        get
        {
            var vc1d = VertexCount1D;
            return (VertexCount1D - 1) * (VertexCount1D - 1) * 2 * 3;
        }
    }
}

class ProcPlane
{
    public static void Gen1(Mesh mesh)
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
    public static void Gen2(Mesh mesh, int xCount, int zCount)
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
    public static void Gen3(Mesh mesh, int xCount, int zCount, float xSize, float zSize)
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
    public static void Gen4(Mesh mesh, int xCount, int zCount, float xSize, float zSize)
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
    public static void Gen5(Mesh mesh, int xCount, int zCount, float xSize, float zSize, Func<float, float, float> height)
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
        for (z = 1; z < zCount - 1; z++)
        {
            for (x = 1; x < xCount - 1; x++)
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
    public static void Gen6(MeshInfo info, Func<float, float, float> height)
    {
        int xCount = info.VertexCount1D;
        int zCount = xCount;
        float xSize = info.size.x;
        float zSize = info.size.y;
        Mesh mesh = info.mesh;
        var verts = info.vertices; // new NativeArray<ExampleVertex>(vertexCount, Allocator.Temp);
        var indices = info.indices; // new NativeArray<ushort>(indiceCount, Allocator.Temp);
        var vertexCount = info.VertexCount2D;
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
