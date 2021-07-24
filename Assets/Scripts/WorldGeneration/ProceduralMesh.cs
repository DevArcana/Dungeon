using System.Collections.Generic;
using UnityEngine;

namespace WorldGeneration
{
    public class ProceduralMesh
    {
        private List<Vector3> _vertices = new List<Vector3>();
        private List<int> _triangles = new List<int>();
        private List<Vector2> _uv = new List<Vector2>();

        public int AddVertex(Vector3 position, Vector2 uv)
        {
            _vertices.Add(position);
            _uv.Add(uv);

            return _vertices.Count - 1;
        }

        public void AddTriangle(int v1, int v2, int v3)
        {
            _triangles.Add(v1);
            _triangles.Add(v2);
            _triangles.Add(v3);
        }

        public Mesh ToMesh()
        {
            var mesh = new Mesh
            {
                vertices = _vertices.ToArray(),
                triangles = _triangles.ToArray(),
                uv = _uv.ToArray()
            };

            mesh.RecalculateNormals();

            return mesh;
        }
    }
}