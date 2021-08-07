using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace World
{
  public class WorldChunkManager : MonoBehaviour
  {
    public Heightmap heightmap;
    public WorldChunk chunkPrefab;
    public int chunkSize = 10;
    public int radius = 1;

    public int chunkX, chunkZ;

    public bool editMode = false;

    private void DestroyChunks()
    {
      var children = transform;
      var destroy = new List<GameObject>(children.childCount);
      destroy.AddRange(from Transform chunk in children select chunk.gameObject);

      foreach (var obj in destroy)
      {
        if (editMode)
        {
          DestroyImmediate(obj);
        }
        else
        {
          Destroy(obj);
        }
      }
    }

    private void BuildChunks()
    {
      for (var z = chunkZ - radius; z <= chunkZ + radius; z++)
      {
        for (var x = chunkX - radius; x <= chunkX + radius; x++)
        {
          chunkPrefab.size = chunkSize;
          var chunk = Instantiate(chunkPrefab, new Vector3(x * chunkSize, 0.0f, z * chunkSize), Quaternion.identity, transform);
          chunk.Rebuild();
        }
      }
    }
    
    public void Rebuild()
    {
      var size = (radius * 2 + 1) * chunkSize;
      var offset = radius * chunkSize;
      heightmap.Generate(size, size, -offset, -offset);
      DestroyChunks();
      BuildChunks();
      editMode = false;
    }

    private void Start()
    {
      Rebuild();
    }
  }
}