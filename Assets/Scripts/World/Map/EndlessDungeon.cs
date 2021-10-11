using UnityEngine;
using World.Map.Generation;
using World.Map.Mesh;

namespace World.Map
{
  public class EndlessDungeon : MonoBehaviour
  {
    public bool regenerate;
    public MapGenerationSettings settings;
    public MeshFilter[] layers;

    public void Generate()
    {
      var generator = new MapGenerator(settings);
      var map = generator.Generate();

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

      for (byte i = 0; i < settings.layers; i++)
      {
        if (i < layers.Length)
        {
          var layer = Instantiate(layers[i], Vector3.up * i, Quaternion.identity, transform);
          var meshProvider = new MapLayerMeshProvider(map, i, i != 0);
          layer.mesh = meshProvider.CreateMesh();
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