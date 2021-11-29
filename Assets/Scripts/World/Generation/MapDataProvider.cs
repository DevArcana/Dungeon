using System.Linq;
using UnityEngine;
using World.Common;
using Random = System.Random;

namespace World.Generation
{
  public class MapDataProvider : MonoBehaviour
  {
    // settings
    public bool regenerate;
    public MapGenerationSettings settings;
    public MapGenerationSettings[] configurations;
    public Material[] layers;
    
    // serialized data
    public HeightMap heightmap;
    public RegionsMap regions;

    public void Generate(bool force = false)
    {
      settings = configurations[UnityEngine.Random.Range(0, configurations.Length)];
      
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
      Populate();
    }

    private void Populate()
    {
      // put player in a random position on the map and generate a staircase
      var allRegions = this.regions.AllRegions().ToArray();
      var random = new Random();

      var region = allRegions[random.Next(allRegions.Length)];
      var pos = region.cells[random.Next(region.cells.Count)];
      while (heightmap[pos] > 0)
      {
        pos = region.cells[random.Next(region.cells.Count)];
      }
      Instantiate(settings.playerPrefab, new Vector3(pos.x, heightmap[pos], pos.y), Quaternion.identity, transform);

      var endRegion = region;
      while (allRegions.Length > 1 && endRegion == region)
      {
        endRegion = allRegions[random.Next(allRegions.Length)];
      }
      pos = endRegion.cells[random.Next(endRegion.cells.Count)];
      while (heightmap[pos] > 0)
      {
        pos = endRegion.cells[random.Next(endRegion.cells.Count)];
      }
      Instantiate(settings.staircasePrefab, new Vector3(pos.x, heightmap[pos], pos.y), Quaternion.identity, transform);
    }

    private void BuildMesh()
    {
      var colors = new SerializableMap<Color>(regions.width, regions.height, Color.white);

      foreach (var region in regions.AllRegions())
      {
        var color = UnityEngine.Random.ColorHSV(0.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f);
        foreach (var cell in region.cells)
        {
          colors[cell] = color;
        }
      }
      
      for (byte i = 0; i <= settings.layers && i < layers.Length; i++)
      {
        var obj = new GameObject($"Layer {i}");
        obj.transform.SetParent(transform);
        obj.transform.localPosition = Vector3.up * i;

        var slice = heightmap.SliceAt(i);
        var meshProvider = new MarchingSquaresMesh(slice, i != 0, colors);
        var mesh = meshProvider.CreateMesh();
          
        obj.AddComponent<MeshRenderer>().material = layers[i];
        obj.AddComponent<MeshFilter>().mesh = mesh;
        obj.AddComponent<MeshCollider>().sharedMesh = mesh;

        if (i > 0)
        {
          obj.AddComponent<Cutout>();
        }
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