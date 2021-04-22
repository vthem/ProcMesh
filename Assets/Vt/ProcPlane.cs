
using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
struct ExampleVertex
{
    public Vector3 pos;
    public Vector2 uv;
}

[Serializable]
public struct MeshLodInfo
{
    public int leftLod;
    public int frontLod;
    public int rightLod;
    public int backLod;

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public static bool operator ==(MeshLodInfo a, MeshLodInfo b)
    {
        return a.leftLod == b.leftLod && a.frontLod == b.frontLod && a.rightLod == b.rightLod && a.backLod == b.backLod;
    }

    public static bool operator !=(MeshLodInfo a, MeshLodInfo b)
    {
        return !(a == b);
    }
}

struct MeshGenerateParameter
{
    public Mesh mesh;
    public NativeArray<ExampleVertex> vertices;
    public NativeArray<ushort> indices;
    public MeshLodInfo lodInfo;
    public IVertexModifier vertexModifier;
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
    public static void Gen6(MeshGenerateParameter gp)
    {
        var vMod = gp.vertexModifier;
        vMod.Initialize();
        int xCount = vMod.VertexCount1D;
        int zCount = xCount;
        Mesh mesh = gp.mesh;
        var verts = gp.vertices; // new NativeArray<ExampleVertex>(vertexCount, Allocator.Temp);
        var indices = gp.indices; // new NativeArray<ushort>(indiceCount, Allocator.Temp);
        var vertexCount = vMod.VertexCount2D;
        var indiceCount = vMod.IndiceCount;
        var lod = vMod.Lod;
        var leftLod = gp.lodInfo.leftLod;
        var frontLod = gp.lodInfo.frontLod;
        var rightLod = gp.lodInfo.rightLod;
        var backLod = gp.lodInfo.backLod;
        // Vector3 Vertex(int x, int z);

        // specify vertex count and layout
        var layout = new[]
        {
            new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3),
            new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2)
        };
        mesh.SetVertexBufferParams(vertexCount, layout);

        // seams first
        // +  +  + -> lod 1
        // +o +o + -> lod 2
        // +oo+oo+ -> lod 3
        // + index can be found with modulus
        // o index are all index not modulo

        // left
        int x = 0;
        int z = 0;
        if (leftLod >= 0 && lod > leftLod)
        { // two passes, adapt seams to left lod
            var nLeftZ = VertexModifier.ComputeVertexCount1D(leftLod);
            var modulus = 1 + (zCount - nLeftZ) / (nLeftZ - 1);
            for (z = 0; z < zCount; z += modulus)
                verts = ComputeVertex(verts, x, xCount, z, vMod/*, xDelta, zDelta, xStart, zStart */);
            for (z = 0; z < zCount; z++)
                verts = WaitedComputeVertexZ(verts, x, xCount, z, modulus);
        }
        else
        { // single pass
            for (z = 0; z < zCount; z++)
                verts = ComputeVertex(verts, x, xCount, z, vMod/*, xDelta, zDelta, xStart, zStart */);
        }

        // front
        z = zCount - 1;
        if (frontLod >= 0 && lod > frontLod)
        { // two passes, adapt seams to front lod
            var nFrontX = VertexModifier.ComputeVertexCount1D(frontLod);
            var modulus = 1 + (xCount - nFrontX) / (nFrontX - 1);
            for (x = 0; x < xCount; x += modulus)
                verts = ComputeVertex(verts, x, xCount, z, vMod/*, xDelta, zDelta, xStart, zStart */);
            for (x = 0; x < xCount; x++)
                verts = WaitedComputeVertexX(verts, x, xCount, z, modulus);
        }
        else
        { // single pass
            for (x = 0; x < xCount; x++)
                verts = ComputeVertex(verts, x, xCount, z, vMod/*, xDelta, zDelta, xStart, zStart */);
        }

        // right
        x = xCount - 1;
        if (rightLod >= 0 && lod > rightLod)
        { // two passes, adapt seams to right lod
            var nRightZ = VertexModifier.ComputeVertexCount1D(rightLod);
            var modulus = 1 + (zCount - nRightZ) / (nRightZ - 1);
            for (z = 0; z < zCount; z += modulus)
                verts = ComputeVertex(verts, x, xCount, z, vMod/*, xDelta, zDelta, xStart, zStart */);
            for (z = 0; z < zCount; z++)
                verts = WaitedComputeVertexZ(verts, x, xCount, z, modulus);
        }
        else
        { // single pass
            for (z = 0; z < zCount; z++)
                verts = ComputeVertex(verts, x, xCount, z, vMod/*, xDelta, zDelta, xStart, zStart */);
        }

        // back
        z = 0;
        if (backLod >= 0 && lod > backLod)
        { // two passes, adapt seams to back lod
            var nBackX = VertexModifier.ComputeVertexCount1D(backLod);
            var modulus = 1 + (xCount - nBackX) / (nBackX - 1);
            for (x = 0; x < xCount; x += modulus)
                verts = ComputeVertex(verts, x, xCount, z, vMod/*, xDelta, zDelta, xStart, zStart */);
            for (x = 0; x < xCount; x++)
                verts = WaitedComputeVertexX(verts, x, xCount, z, modulus);
        }
        else
        { // single pass
            for (x = 0; x < xCount; x++)
                verts = ComputeVertex(verts, x, xCount, z, vMod/*, xDelta, zDelta, xStart, zStart */);
        }

        // center
        for (z = 1; z < zCount - 1; z++)
            for (x = 1; x < xCount - 1; x++)
                verts = ComputeVertex(verts, x, xCount, z, vMod/*, xDelta, zDelta, xStart, zStart */);

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
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }

    private static NativeArray<ExampleVertex> WaitedComputeVertexX(NativeArray<ExampleVertex> verts, int x, int xCount, int z, int modulus)
    {
        var i = x + z * xCount;
        var remainder = i % modulus;
        if (remainder == 0)
            return verts;
        var prevKnown = (x - remainder) + z * xCount; // i - remainder
        var nextKnown = (x - remainder + modulus) + z * xCount; // i - remainder + modulus
        verts[i] = new ExampleVertex {
            pos = verts[prevKnown].pos + (verts[nextKnown].pos - verts[prevKnown].pos) * remainder / (float)modulus,
            uv = new Vector2(x / (float)(xCount - 1), z / (float)(xCount - 1))
        };
        return verts;
    }

    private static NativeArray<ExampleVertex> WaitedComputeVertexZ(NativeArray<ExampleVertex> verts, int x, int xCount, int z, int modulus)
    {
        var i = x + z * xCount;
        var remainder = i % modulus;
        if (remainder == 0)
            return verts;
        var prevKnown = x + (z - remainder) * xCount; // i - remainder
        var nextKnown = x + (z - remainder + modulus) * xCount; // i - remainder + modulus
        verts[i] = new ExampleVertex {
            pos = verts[prevKnown].pos + (verts[nextKnown].pos - verts[prevKnown].pos) * remainder / (float)modulus,
            uv = new Vector2(x / (float)(xCount - 1), z / (float)(xCount - 1))
        };
        return verts;
    }

    private static NativeArray<ExampleVertex> ComputeVertex(NativeArray<ExampleVertex> verts, int x, int xCount, int z, IVertexModifier vertexModifier/*, float xDelta, float zDelta, float xStart, float zStart*/)
    {
        var i = x + z * xCount;
        //var xPos = xStart + x * xDelta;
        //var zPos = zStart + z * zDelta;
        verts[i] = new ExampleVertex {
            pos = vertexModifier.Vertex(x, z),
            uv = new Vector2(x / (float)(xCount-1), z / (float)(xCount-1))
        };
        return verts;
    }
}
