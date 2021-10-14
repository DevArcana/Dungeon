using System;
using UnityEngine;
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
    public Heightmap heightmap;

    public void Generate(bool force = false)
    {
      if (!regenerate && !force)
      {
        return;
      }
      
      var generator = new MapGenerator(settings);
      var map = generator.Generate();

      heightmap = map;
      
      Clear();
      BuildMesh(map);
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
      for (var y = 0; y < heightmap.height; y++)
      {
        for (var x = 0; x < heightmap.width; x++)
        {
          Gizmos.DrawCube(new Vector3(x, heightmap[x, y], y), Vector3.one * 0.5f);
        }
      }
    }
  }
}