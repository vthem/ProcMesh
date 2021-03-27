using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class ProcPlaneBehaviour : MonoBehaviour
{
    [Range(0, 10)]
    public int lod = 0;
    public Vector2 size = Vector2.one;

    private MeshInfo meshInfo;

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
            ProcPlane.Gen6(meshInfo, Perlin);
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
        meshInfo.lod = lod;

        meshInfo.vertices = new NativeArray<ExampleVertex>(meshInfo.VertexCount2D, Allocator.Persistent);
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
        return meshInfo.size == size && meshInfo.lod == lod;
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
}
