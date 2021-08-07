using UnityEngine;

namespace World.Mesh
{
    public class CaveBuilder
    {
        private readonly ProceduralMesh _mesh = new ProceduralMesh();

        public UnityEngine.Mesh Build()
        {
            return _mesh.ToMesh();
        }

        private float GetHorizontalDistance(Vector3 from, Vector3 to)
        {
            var dx = to.x - from.x;
            var dz = to.z - from.z;

            return Mathf.Sqrt(dx*dx + dz*dz);
        }

        public void AddWall(Vector3 from, Vector3 to)
        {
            var h = to.y - from.y;
            var d = GetHorizontalDistance(from, to);

            var dh = Vector3.up * h;
            
            var v1 = _mesh.AddVertex(from, new Vector2(0, 0));
            var v2 = _mesh.AddVertex(from + dh, new Vector2(0, h));
            var v3 = _mesh.AddVertex(to, new Vector2(d, h));
            var v4 = _mesh.AddVertex(to - dh, new Vector2(d, 0));

            _mesh.AddTriangle(v1, v2, v3);
            _mesh.AddTriangle(v1, v3, v4);
        }

        public void AddFloor(Vector3 from, Vector3 to)
        {
            var x = to.x - from.x;
            var z = to.z - from.z;
            var dx = Vector3.right * x;
            var dz = Vector3.forward * z;
            
            var v1 = _mesh.AddVertex(from, new Vector2(from.x, from.z));
            var v2 = _mesh.AddVertex(from + dz, new Vector2(from.x, from.z + z));
            var v3 = _mesh.AddVertex(from + dz + dx, new Vector2(from.x + x, from.z + z));
            var v4 = _mesh.AddVertex(from + dx, new Vector2(from.x + x, from.z));

            _mesh.AddTriangle(v1, v2, v3);
            _mesh.AddTriangle(v1, v3, v4);
        }
    }
}