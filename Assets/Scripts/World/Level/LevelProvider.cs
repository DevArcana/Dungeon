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

    public void Generate()
    {
      var generator = new MapGenerator(settings);
      var map = generator.Generate();

      heightmap = map;
      
      Clear();
      BuildMesh(map);
    }

    private void BuildMesh(Common.Heightmap heightmap)
    {
      for (byte i = 0; i < settings.layers && i < layers.Length; i++)
      {
        var obj = new GameObject($"Layer {i}");
        obj.transform.SetParent(transform);
        obj.transform.localPosition = Vector3.up * i;
          
        var meshProvider = new MapLayerMeshProvider(heightmap, i, i != 0);
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
      if (regenerate)
      {
        Generate();
      }
    }
  }
}