using System.Collections.Generic;
using UnityEngine;

namespace World.Mesh
{
    public class ProceduralMesh
    {
        private readonly List<Vector3> _vertices = new List<Vector3>();
        private readonly List<int> _triangles = new List<int>();
        private readonly List<Vector2> _uv = new List<Vector2>();

        private readonly Dictionary<string, int> _cache = new Dictionary<string, int>();

        public int AddVertex(Vector3 position, Vector2 uv)
        {
            // primitive cache, cavemen approved
            var key = $"{position}{uv}";
            if (_cache.TryGetValue(key, out var vertex))
            {
                return vertex;
            }
            
            _vertices.Add(position);
            _uv.Add(uv);

            vertex = _vertices.Count - 1;
            _cache[key] = vertex;
            
            return vertex;
        }

        public void AddTriangle(int v1, int v2, int v3)
        {
            _triangles.Add(v1);
            _triangles.Add(v2);
            _triangles.Add(v3);
        }

        public UnityEngine.Mesh ToMesh()
        {
            Debug.Log($"Mesh created containing {_vertices.Count} vertices and {+_triangles.Count / 3} triangles.");
            
            var mesh = new UnityEngine.Mesh
            {
                vertices = _vertices.ToArray(),
                triangles = _triangles.ToArray(),
                uv = _uv.ToArray()
            };
            
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            mesh.RecalculateTangents();
            
            mesh.Optimize();

            return mesh;
        }
    }
}