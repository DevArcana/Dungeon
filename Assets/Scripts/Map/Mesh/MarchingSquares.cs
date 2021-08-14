using System.Collections.Generic;
using Map.Generation;
using UnityEngine;

namespace Map.Mesh
{
  [RequireComponent(typeof(MeshFilter))]
  public class MarchingSquares : MonoBehaviour
  {
    public int height;
    public MapGenerator map;
    private MeshFilter _meshFilter;

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
      
      node.vertexIndex = _vertices.Count;
      _vertices.Add(position);
      _uvs.Add(new Vector2(position.x, position.z));
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

    #endregion

    private void Start()
    {
      _meshFilter = GetComponent<MeshFilter>();

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
              Fill(bottomLeft.corner, bottomLeft.up, bottomLeft.right);
              break;
            case 2:
              Fill(bottomLeft.up, topLeft.corner, topLeft.right);
              break;
            case 4:
              Fill(topLeft.right, topRight.corner, bottomRight.up);
              break;
            case 8:
              Fill(bottomRight.up, bottomRight.corner, bottomLeft.right);
              break;
            
            case 3:
              Fill(bottomLeft.corner, topLeft.corner, topLeft.right, bottomLeft.right);
              break;
            case 6:
              Fill(topLeft.corner, topRight.corner, bottomRight.up, bottomLeft.up);
              break;
            case 12:
              Fill(topRight.corner, bottomRight.corner, bottomLeft.right, topLeft.right);
              break;
            case 9:
              Fill(bottomLeft.corner, bottomLeft.up, bottomRight.up, bottomRight.corner);
              break;
            
            case 14:
              Fill(topRight.corner, bottomRight.corner, bottomLeft.right, bottomLeft.up, topLeft.corner);
              break;
            case 13:
              Fill(bottomRight.corner, bottomLeft.corner, bottomLeft.up, topLeft.right, topRight.corner);
              break;
            case 11:
              Fill(bottomLeft.corner, topLeft.corner, topLeft.right, bottomRight.up, bottomRight.corner);
              break;
            case 7:
              Fill(topLeft.corner, topRight.corner, bottomRight.up, bottomLeft.right, bottomLeft.corner);
              break;
            
            case 5:
              Fill(bottomLeft.corner, bottomLeft.up, topLeft.right, topRight.corner, bottomRight.up, bottomLeft.right);
              break;
            
            case 10:
              Fill(bottomRight.corner, bottomLeft.right, bottomLeft.up, topLeft.corner, topLeft.right, bottomRight.up);
              break;

            case 15:
              Fill(bottomLeft.corner, topLeft.corner, topRight.corner, bottomRight.corner);
              break;
          }
        }
      }

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
    }
  }
}