using UnityEngine;
using WorldGeneration;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class GenerationTester : MonoBehaviour
{
    private MeshRenderer _meshRenderer;
    private MeshFilter _meshFilter;

    private void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshFilter = GetComponent<MeshFilter>();
    }

    public void Generate()
    {
        Debug.Log("Starting generation...");
        Start();
        
        var mesh = new ProceduralMesh();
        DrawWalls(mesh, new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(2, 2), new Vector2(3, 2), new Vector2(3, -1), new Vector2(0, -1), new Vector2(0, 0));
        
        _meshFilter.mesh = mesh.ToMesh();
        Debug.Log("Finished.");
    }

    private static void DrawWalls(ProceduralMesh mesh, params Vector2[] points)
    {
        for (var i = 0; i < points.Length - 1; i++)
        {
            var start = points[i];
            var end = points[i + 1];
            
            DrawWall(mesh, new Vector3(start.x, 0, start.y), new Vector3(end.x, 1, end.y));
        }
    }

    private static void DrawWall(ProceduralMesh mesh, Vector3 start, Vector3 end)
    {
        var startX = Mathf.Sqrt(start.x * start.x + start.z * start.z);
        var endX = Mathf.Sqrt(end.x * end.x + end.z * end.z);
        
        var v1 = mesh.AddVertex(start, new Vector2(startX, start.y));
        var v2 = mesh.AddVertex(new Vector3(start.x, end.y, start.z), new Vector2(startX, end.y));
        var v3 = mesh.AddVertex(end, new Vector2(endX, end.y));
        var v4 = mesh.AddVertex(new Vector3(end.x, start.y, end.z), new Vector2(endX, start.y));
        
        mesh.AddTriangle(v1, v2, v3);
        mesh.AddTriangle(v3, v4, v1);
    }
}
