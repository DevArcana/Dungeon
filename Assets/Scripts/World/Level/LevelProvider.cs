using System;
using UnityEngine;
using Utils;
using World.Level.Common;
using World.Level.Generation;
using World.Level.Mesh;

namespace World.Level
{
  public class LevelProvider : MonoBehaviour
  {
    public bool regenerate;
    public MapGenerationSettings settings;
    public Material[] layers;
    public MapGenerator generator;

    public void Generate(bool force = false)
    {
      if (!regenerate && !force)
      {
        return;
      }
      
      generator = new MapGenerator(settings);
      generator.Generate();

      Clear();
      BuildMesh(generator.heightmap);
    }

    private void BuildMesh(Heightmap map)
    {
      for (byte i = 0; i <= settings.layers && i < layers.Length; i++)
      {
        var obj = new GameObject($"Layer {i}");
        obj.transform.SetParent(transform);
        obj.transform.localPosition = Vector3.up * i;
          
        var meshProvider = new MapLayerMeshProvider(map, i, i != 0);
        var mesh = meshProvider.CreateMesh();
          
        obj.AddComponent<MeshRenderer>().material = layers[i];
        obj.AddComponent<MeshFilter>().mesh = mesh;
        obj.AddComponent<MeshCollider>().sharedMesh = mesh;
      }
    }

    private void Clear()
    {
      var childCount = transform.childCount;
      while (childCount > 0)
      {
        childCount--;
        var obj = transform.GetChild(childCount).gameObject;

        if (Application.isEditor)
        {
          DestroyImmediate(obj);
        }
        else
        {
          Destroy(obj);
        }
      }
    }

    private void Start()
    {
      Generate();
    }

    private void OnDrawGizmos()
    {
      for (var y = 0; y < generator.heightmap.height; y++)
      {
        for (var x = 0; x < generator.heightmap.width; x++)
        {
          Gizmos.DrawCube(new Vector3(x, generator.heightmap[x, y], y), Vector3.one * 0.5f);
        }
      }

      Gizmos.color = Color.red;
      foreach (var region in generator.regionBuilder.Regions)
      {
        foreach (var cell in region.outline)
        {
          Gizmos.DrawCube(new Vector3(cell.x, generator.heightmap[cell.x, cell.y], cell.y), Vector3.one * 0.75f);
        }
      }

      Gizmos.color = Color.green;

      foreach (var connection in generator.regionBuilder.Connections)
      {
        Gizmos.DrawLine(new Vector3(connection.PosA.x, generator.heightmap[connection.PosA.x, connection.PosA.y], connection.PosA.y), new Vector3(connection.PosB.x, generator.heightmap[connection.PosB.x, connection.PosB.y], connection.PosB.y));
      }
    }
  }
}