﻿using UnityEngine;
using World.Common;

namespace World.Generation
{
  public class MapDataProvider : MonoBehaviour
  {
    // settings
    public bool regenerate;
    public MapGenerationSettings settings;
    public Material[] layers;
    
    // serialized data
    public HeightMap heightmap;
    public SerializableMap<int> regions;

    public void Generate(bool force = false)
    {
      if (!regenerate && !force)
      {
        return;
      }

      var generator = new MapGenerator(settings);
      var result = generator.Generate();
      
      heightmap = result.heightmap;
      regions = result.regions;

      Clear();
      BuildMesh();
    }

    private void BuildMesh()
    {
      for (byte i = 0; i <= settings.layers && i < layers.Length; i++)
      {
        var obj = new GameObject($"Layer {i}");
        obj.transform.SetParent(transform);
        obj.transform.localPosition = Vector3.up * i;
          
        var meshProvider = new MarchingSquaresMesh(heightmap.SliceAt(i), i != 0);
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

    private static readonly Color[] Colors = {
      new Color(0, 0, 1),
      new Color(0, 1, 0),
      new Color(1, 0, 0),
      new Color(1, 1, 0),
      new Color(1, 0, 1),
      new Color(0, 1, 1)
    };

    private void OnDrawGizmos()
    {
      for (var y = 0; y < heightmap.height; y++)
      {
        for (var x = 0; x < heightmap.width; x++)
        {
          Gizmos.DrawCube(new Vector3(x, heightmap[x, y], y), Vector3.one * 0.5f);
        }
      }

      if (regions != null)
      {
        for (var y = 0; y < regions.height; y++)
        {
          for (var x = 0; x < regions.width; x++)
          {
            var region = regions[x, y];
            if (region != -1)
            {
              Gizmos.color = Colors[region % Colors.Length];
              Gizmos.DrawCube(new Vector3(x, heightmap[x, y], y), Vector3.one * 0.75f);
            }
          }
        }
      }
    }
  }
}