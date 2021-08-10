using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace World
{
  [RequireComponent(typeof(MeshFilter))]
  public class Chunk : MonoBehaviour
  {
    public int size, height;
    public VoxelType type;
    public WorldDataProvider dataProvider;

    public MeshFilter meshFilter;
    public MeshCollider meshCollider;

    private int _originX, _originZ;
    
    private readonly List<Vector3> _vertices = new List<Vector3>();
    private readonly List<Vector2> _uvs = new List<Vector2>();
    private readonly List<int> _triangles = new List<int>();

    private void ClearMesh()
    {
      _vertices.Clear();
      _uvs.Clear();
      _triangles.Clear();
    }

    private int AddVertex(Vector3 pos, Vector2 uv)
    {
      _vertices.Add(pos);
      _uvs.Add(uv);
      return _vertices.Count - 1;
    }

    private void BuildTop(int x, int y, int z)
    {
      if (dataProvider.GetVoxel(x + _originX, y + 1, z + _originZ).type == VoxelType.Air)
      {
        var pos = new Vector3(x, y + 1, z);
        var v1 = AddVertex(pos, new Vector2(0, 0));
        var v2 = AddVertex(pos + Vector3.forward, new Vector2(0, 1));
        var v3 = AddVertex(pos + Vector3.forward + Vector3.right, new Vector2(1, 1));
        var v4 = AddVertex(pos + Vector3.right, new Vector2(1, 0));
        
        _triangles.Add(v1);
        _triangles.Add(v2);
        _triangles.Add(v3);
        
        _triangles.Add(v1);
        _triangles.Add(v3);
        _triangles.Add(v4);
      }
    }
    
    private void BuildBackward(int x, int y, int z)
    {
      if (dataProvider.GetVoxel(x + _originX, y, z + _originZ - 1).type == VoxelType.Air)
      {
        var pos = new Vector3(x, y, z);
        var v1 = AddVertex(pos, new Vector2(0, 0));
        var v2 = AddVertex(pos + Vector3.up, new Vector2(0, 1));
        var v3 = AddVertex(pos + Vector3.up + Vector3.right, new Vector2(1, 1));
        var v4 = AddVertex(pos + Vector3.right, new Vector2(1, 0));
        
        _triangles.Add(v1);
        _triangles.Add(v2);
        _triangles.Add(v3);
        
        _triangles.Add(v1);
        _triangles.Add(v3);
        _triangles.Add(v4);
      }
    }
    
    private void BuildForward(int x, int y, int z)
    {
      if (dataProvider.GetVoxel(x + _originX, y, z + _originZ + 1).type == VoxelType.Air)
      {
        var pos = new Vector3(x, y, z + 1);
        var v4 = AddVertex(pos, new Vector2(1, 0));
        var v3 = AddVertex(pos + Vector3.up, new Vector2(1, 1));
        var v2 = AddVertex(pos + Vector3.up + Vector3.right, new Vector2(0, 1));
        var v1 = AddVertex(pos + Vector3.right, new Vector2(0, 0));
        
        _triangles.Add(v1);
        _triangles.Add(v2);
        _triangles.Add(v3);
        
        _triangles.Add(v1);
        _triangles.Add(v3);
        _triangles.Add(v4);
      }
    }
    
    private void BuildRight(int x, int y, int z)
    {
      if (dataProvider.GetVoxel(x + _originX + 1, y, z + _originZ).type == VoxelType.Air)
      {
        var pos = new Vector3(x + 1, y, z);
        var v1 = AddVertex(pos, new Vector2(0, 0));
        var v2 = AddVertex(pos + Vector3.up, new Vector2(0, 1));
        var v3 = AddVertex(pos + Vector3.up + Vector3.forward, new Vector2(1, 1));
        var v4 = AddVertex(pos + Vector3.forward, new Vector2(1, 0));
        
        _triangles.Add(v1);
        _triangles.Add(v2);
        _triangles.Add(v3);
        
        _triangles.Add(v1);
        _triangles.Add(v3);
        _triangles.Add(v4);
      }
    }
    
    private void BuildLeft(int x, int y, int z)
    {
      if (dataProvider.GetVoxel(x + _originX - 1, y, z + _originZ).type == VoxelType.Air)
      {
        var pos = new Vector3(x, y, z);
        var v4 = AddVertex(pos, new Vector2(1, 0));
        var v3 = AddVertex(pos + Vector3.up, new Vector2(1, 1));
        var v2 = AddVertex(pos + Vector3.up + Vector3.forward, new Vector2(0, 1));
        var v1 = AddVertex(pos + Vector3.forward, new Vector2(0, 0));
        
        _triangles.Add(v1);
        _triangles.Add(v2);
        _triangles.Add(v3);
        
        _triangles.Add(v1);
        _triangles.Add(v3);
        _triangles.Add(v4);
      }
    }
    
    private void BuildBottom(int x, int y, int z)
    {
      if (dataProvider.GetVoxel(x + _originX, y - 1, z + _originZ).type == VoxelType.Air)
      {
        var pos = new Vector3(x, y, z);
        var v1 = AddVertex(pos, new Vector2(0, 0));
        var v2 = AddVertex(pos + Vector3.forward, new Vector2(0, 1));
        var v3 = AddVertex(pos + Vector3.forward + Vector3.right, new Vector2(1, 1));
        var v4 = AddVertex(pos + Vector3.right, new Vector2(1, 0));
        
        _triangles.Add(v1);
        _triangles.Add(v4);
        _triangles.Add(v3);
        
        _triangles.Add(v1);
        _triangles.Add(v3);
        _triangles.Add(v2);
      }
    }

    private IEnumerator Build()
    {
      ClearMesh();
      
      var position = transform.position;
      _originX = (int)(position.x);
      _originZ = (int)(position.z);

      for (var y = -1; y < height; y++)
      {
        for (var z = 0; z < size; z++)
        {
          for (var x = 0; x < size; x++)
          {
            var voxel = dataProvider.GetVoxel(x + _originX, y, z + _originZ);

            if (voxel.type != type)
            {
              continue;
            }
            
            BuildTop(x, y, z);
            BuildBottom(x, y, z);
            BuildForward(x, y, z);
            BuildBackward(x, y, z);
            BuildLeft(x, y, z);
            BuildRight(x, y, z);
          }
        }
        yield return null;
      }

      var mesh = new Mesh()
      {
        vertices = _vertices.ToArray(),
        triangles = _triangles.ToArray(),
        uv = _uvs.ToArray()
      };
      
      mesh.RecalculateNormals();
      mesh.Optimize();

      meshFilter.sharedMesh = mesh;
      meshCollider.sharedMesh = mesh;
    }

    public void Rebuild()
    {
      StartCoroutine(Build());
    }
  }
}