using System.Collections.Generic;
using Map.Generation;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Map.Mesh
{
  [RequireComponent(typeof(MeshFilter))]
  [RequireComponent(typeof(MeshCollider))]
  public class MarchingSquares : MonoBehaviour
  {
    public int height;
    public bool generateWalls = true;
    public MapGenerator map;
    private MeshFilter _meshFilter;
    private MeshCollider _collider;
    
    private readonly List<Vector3> _vertices = new List<Vector3>();
    private readonly List<Vector2> _uvs = new List<Vector2>();
    private readonly List<int> _triangles = new List<int>();

    private class VoxelNode
    {
      public int vertexIndex;
      public Vector3 position;

      public VoxelNode(Vector3 position)
      {
        this.vertexIndex = -1;
        this.position = position;
      }
    }

    private class Voxel
    {
      public readonly bool filled;
      
      public VoxelNode up;
      public VoxelNode corner;
      public VoxelNode right;

      public Voxel(Vector3 position, bool filled)
      {
        this.filled = filled;
        up = new VoxelNode(position + Vector3.forward * 0.5f);
        corner = new VoxelNode(position);
        right = new VoxelNode(position + Vector3.right * 0.5f);
      }
    }

    private int _width;
    private int _height;
    private Voxel[,] _voxels;

    #region MeshManipulation

    private void Clear()
    {
      _vertices.Clear();
      _uvs.Clear();
      _triangles.Clear();
      
      var data = map.GetMapData();

      _width = data.GetLength(0);
      _height = data.GetLength(1);

      _voxels = new Voxel[_width, _height];

      for (var y = 0; y < _height; y++)
      {
        for (var x = 0; x < _width; x++)
        {
          _voxels[x, y] = new Voxel(new Vector3(x, 0.0f, y), data[x, y] == height);
        }
      }
    }

    private void InitializeVertex(VoxelNode node)
    {
      if (node.vertexIndex != -1)
      {
        return;
      }

      var position = node.position;
      node.vertexIndex = AddVertex(position, new Vector2(position.x, position.z));
    }

    private void Fill(params VoxelNode[] nodes)
    {
      var a = nodes[0];
      InitializeVertex(a);
      
      for (var i = 1; i < nodes.Length - 1; i++)
      {
        var b = nodes[i];
        var c = nodes[i + 1];
        
        InitializeVertex(b);
        InitializeVertex(c);
        
        _triangles.Add(a.vertexIndex);
        _triangles.Add(b.vertexIndex);
        _triangles.Add(c.vertexIndex);
      }
    }

    private int AddVertex(Vector3 position, Vector2 uv)
    {
      _vertices.Add(SpaceDistortion.Distort(position, transform.position));
      _uvs.Add(uv);
      return _vertices.Count - 1;
    }

    private void AddWall(VoxelNode from, VoxelNode to)
    {
      if (!generateWalls)
      {
        return;
      }
      
      var dir = (to.position - from.position).normalized;

      var topLeft = AddVertex(from.position, new Vector2(dir.x * from.position.x + dir.z * from.position.z, 1.0f));
      var topRight = AddVertex(to.position, new Vector2(dir.x * to.position.x + dir.z * to.position.z, 1.0f));
      var bottomRight = AddVertex(to.position + Vector3.down, new Vector2(dir.x * (to.position + Vector3.down).x + dir.z * (to.position + Vector3.down).z, 0.0f));
      var bottomLeft = AddVertex(from.position + Vector3.down, new Vector2(dir.x * (from.position + Vector3.down).x + dir.z * (from.position + Vector3.down).z, 0.0f));
      
      _triangles.Add(topLeft);
      _triangles.Add(topRight);
      _triangles.Add(bottomRight);
      
      _triangles.Add(topLeft);
      _triangles.Add(bottomRight);
      _triangles.Add(bottomLeft);
    }

    #endregion

    private void Start()
    {
      _meshFilter = GetComponent<MeshFilter>();
      _collider = GetComponent<MeshCollider>();

      Rebuild();
    }

    private void Rebuild()
    {
      Clear();
      
      for (var y = 0; y < _height - 1; y++)
      {
        for (var x = 0; x < _width - 1; x++)
        {
          var bottomLeft = _voxels[x, y]; // 0001
          var topLeft = _voxels[x, y + 1]; // 0010
          var topRight = _voxels[x + 1, y + 1]; // 0100
          var bottomRight = _voxels[x + 1, y]; // 1000

          var voxel = 0;

          voxel = bottomLeft.filled ? voxel | 1 : voxel;
          voxel = topLeft.filled ? voxel | 2 : voxel;
          voxel = topRight.filled ? voxel | 4 : voxel;
          voxel = bottomRight.filled ? voxel | 8 : voxel;

          switch (voxel)
          {
            case 1:
              AddWall(bottomLeft.right, bottomLeft.up);
              Fill(bottomLeft.corner, bottomLeft.up, bottomLeft.right);
              break;
            case 2:
              AddWall(bottomLeft.up, topLeft.right);
              Fill(bottomLeft.up, topLeft.corner, topLeft.right);
              break;
            case 4:
              AddWall(topLeft.right, bottomRight.up);
              Fill(topLeft.right, topRight.corner, bottomRight.up);
              break;
            case 8:
              AddWall(bottomRight.up, bottomLeft.right);
              Fill(bottomRight.up, bottomRight.corner, bottomLeft.right);
              break;
            
            case 3:
              AddWall(bottomLeft.right, topLeft.right);
              Fill(bottomLeft.corner, topLeft.corner, topLeft.right, bottomLeft.right);
              break;
            case 6:
              AddWall(bottomLeft.up, bottomRight.up);
              Fill(topLeft.corner, topRight.corner, bottomRight.up, bottomLeft.up);
              break;
            case 12:
              AddWall(topLeft.right, bottomLeft.right);
              Fill(topRight.corner, bottomRight.corner, bottomLeft.right, topLeft.right);
              break;
            case 9:
              AddWall(bottomRight.up, bottomLeft.up);
              Fill(bottomLeft.corner, bottomLeft.up, bottomRight.up, bottomRight.corner);
              break;
            
            case 14:
              AddWall(bottomLeft.up, bottomLeft.right);
              Fill(topRight.corner, bottomRight.corner, bottomLeft.right, bottomLeft.up, topLeft.corner);
              break;
            case 13:
              AddWall(topLeft.right, bottomLeft.up);
              Fill(bottomRight.corner, bottomLeft.corner, bottomLeft.up, topLeft.right, topRight.corner);
              break;
            case 11:
              AddWall(bottomRight.up, topLeft.right);
              Fill(bottomLeft.corner, topLeft.corner, topLeft.right, bottomRight.up, bottomRight.corner);
              break;
            case 7:
              AddWall(bottomLeft.right, bottomRight.up);
              Fill(topLeft.corner, topRight.corner, bottomRight.up, bottomLeft.right, bottomLeft.corner);
              break;
            
            case 5:
              AddWall(topLeft.right, bottomLeft.up);
              AddWall(bottomLeft.right, bottomRight.up);
              Fill(bottomLeft.corner, bottomLeft.up, topLeft.right, topRight.corner, bottomRight.up, bottomLeft.right);
              break;
            
            case 10:
              AddWall(bottomLeft.up, bottomLeft.right);
              AddWall(bottomRight.up, topLeft.right);
              Fill(bottomRight.corner, bottomLeft.right, bottomLeft.up, topLeft.corner, topLeft.right, bottomRight.up);
              break;

            case 15:
              Fill(bottomLeft.corner, topLeft.corner, topRight.corner, bottomRight.corner);
              break;
          }
        }
      }
      
      // boundary walls
      // TODO: fix seams
      AddWall(_voxels[0, 0].corner, _voxels[_width - 1, 0].corner);
      AddWall(_voxels[_width - 1, 0].corner, _voxels[_width - 1, _height - 1].corner);
      AddWall(_voxels[_width - 1, _height - 1].corner, _voxels[0, _height - 1].corner);
      AddWall(_voxels[0, _height - 1].corner, _voxels[0, 0].corner);

      var mesh = new UnityEngine.Mesh
      {
        vertices = _vertices.ToArray(),
        uv = _uvs.ToArray(),
        triangles = _triangles.ToArray()
      };

      mesh.RecalculateNormals();
      mesh.RecalculateTangents();
      mesh.Optimize();

      _meshFilter.mesh = mesh;
      _collider.sharedMesh = mesh;
    }
  }
}