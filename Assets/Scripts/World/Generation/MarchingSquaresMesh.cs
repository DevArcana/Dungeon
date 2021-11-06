using System.Collections.Generic;
using UnityEngine;
using Utils;
using World.Common;

namespace World.Generation
{
  public class MarchingSquaresMesh
  {
    private readonly bool _generateWalls;

    private readonly SerializableMap<bool> _map;

    private readonly SerializableMap<Color> _colorMap;

    private readonly List<Vector3> _vertices = new List<Vector3>();

    private readonly List<Color> _colors = new List<Color>();

    private readonly List<Vector2> _uvs = new List<Vector2>();

    private readonly List<int> _triangles = new List<int>();

    public MarchingSquaresMesh(SerializableMap<bool> map, bool generateWalls, SerializableMap<Color> colorMap)
    {
      _generateWalls = generateWalls;
      _colorMap = colorMap;
      _map = map;
    }

    private class VoxelNode
    {
      public int vertexIndex;
      public readonly Vector3 position;
      public readonly Color color;

      public VoxelNode(Vector3 position, Color color)
      {
        vertexIndex = -1;
        this.position = position;
        this.color = color;
      }
    }

    private class Voxel
    {
      public readonly bool filled;
      
      public readonly VoxelNode up;
      public readonly VoxelNode corner;
      public readonly VoxelNode right;

      public Voxel(Vector3 position, bool filled, Color color)
      {
        this.filled = filled;

        var pos = MapUtils.ToMapPos(position);
        up = new VoxelNode(position + Vector3.forward * 0.5f, color);
        corner = new VoxelNode(position, color);
        right = new VoxelNode(position + Vector3.right * 0.5f, color);
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

      _width = _map.width;
      _height = _map.height;

      _voxels = new Voxel[_width + 2, _height + 2];

      for (var y = -1; y <= _height; y++)
      {
        for (var x = -1; x <= _width; x++)
        {
          _voxels[x + 1, y + 1] = new Voxel(new Vector3(x, 0.0f, y), !_map.WithinBounds(x, y) || _map[x, y], _colorMap.WithinBounds(x, y) ? _colorMap[x, y] : Color.white);
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
      node.vertexIndex = AddVertex(position, new Vector2(position.x, position.z), node.color);
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

    private int AddVertex(Vector3 position, Vector2 uv, Color color)
    {
      _vertices.Add(position);
      _colors.Add(color);
      _uvs.Add(uv);
      return _vertices.Count - 1;
    }

    private void AddWall(VoxelNode from, VoxelNode to)
    {
      if (!_generateWalls)
      {
        return;
      }
      
      var dir = (to.position - from.position).normalized;

      var topLeft = AddVertex(from.position, new Vector2(dir.x * from.position.x + dir.z * from.position.z, 1.0f), Color.white);
      var topRight = AddVertex(to.position, new Vector2(dir.x * to.position.x + dir.z * to.position.z, 1.0f), Color.white);
      var bottomRight = AddVertex(to.position + Vector3.down, new Vector2(dir.x * (to.position + Vector3.down).x + dir.z * (to.position + Vector3.down).z, 0.0f), Color.white);
      var bottomLeft = AddVertex(from.position + Vector3.down, new Vector2(dir.x * (from.position + Vector3.down).x + dir.z * (from.position + Vector3.down).z, 0.0f), Color.white);
      
      _triangles.Add(topLeft);
      _triangles.Add(topRight);
      _triangles.Add(bottomRight);
      
      _triangles.Add(topLeft);
      _triangles.Add(bottomRight);
      _triangles.Add(bottomLeft);
    }

    #endregion

    public Mesh CreateMesh()
    {
      Clear();
      
      for (var ty = -1; ty <= _height - 1; ty++)
      {
        for (var tx = -1; tx <= _width - 1; tx++)
        {
          var x = tx + 1;
          var y = ty + 1;
          
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
      
      AddWall(_voxels[0, 0].corner, _voxels[_width + 1, 0].corner);
      AddWall(_voxels[_width + 1, 0].corner, _voxels[_width + 1, _height + 1].corner);
      AddWall(_voxels[_width + 1, _height + 1].corner, _voxels[0, _height + 1].corner);
      AddWall(_voxels[0, _height + 1].corner, _voxels[0, 0].corner);

      var mesh = new Mesh
      {
        vertices = _vertices.ToArray(),
        uv = _uvs.ToArray(),
        triangles = _triangles.ToArray(),
        colors = _colors.ToArray()
      };

      mesh.RecalculateNormals();
      mesh.RecalculateTangents();
      mesh.Optimize();

      return mesh;
    }
  }
}