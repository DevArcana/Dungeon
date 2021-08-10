using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace World
{
  public class ChunkProvider : MonoBehaviour
  {
    public WorldDataProvider provider;
    public Chunk chunkPrefab;
    public int chunkSize = 16;
    public int chunkHeight = 4;
    public int radius = 1;

    private readonly Dictionary<string, Chunk> _chunks = new Dictionary<string, Chunk>();
    private int _chunkX, _chunkZ;
    private bool _loading;

    private IEnumerator LoadTerrain(int chunkX, int chunkZ)
    {
      _loading = true;
      Debug.Log("Loading chunks...");
      
      for (var z = chunkZ - radius; z <= chunkZ + radius; z++)
      {
        for (var x = chunkX - radius; x <= chunkX + radius; x++)
        {
          var key = $"{x}:{z}";

          if (!_chunks.ContainsKey(key) || _chunks[key] == null)
          {
            var chunk = Instantiate(chunkPrefab, new Vector3(x * chunkSize, 0.0f, z * chunkSize), Quaternion.identity, transform);
            _chunks[key] = chunk;
            
            chunk.dataProvider = provider;
            chunk.size = chunkSize;
            chunk.height = chunkHeight;
            chunk.Rebuild();
            
            yield return null;
          }
        }
      }

      var dx = _chunkX + Math.Sign(_chunkX - chunkX) * radius;

      if (dx != _chunkX)
      {
        for (var i = _chunkZ - radius; i <= _chunkZ + radius; i++)
        {
          var key = $"{dx}:{i}";
          if (_chunks.TryGetValue(key, out var chunk))
          {
            _chunks.Remove(key);
            if (chunk != null)
            {
              Destroy(chunk.gameObject);
            }

            yield return null;
          }
        }
      }
      
      var dz = _chunkZ + Math.Sign(_chunkZ - chunkZ) * radius;
      
      if (dz != _chunkZ)
      {
        for (var i = _chunkX - radius; i <= _chunkX + radius; i++)
        {
          var key = $"{i}:{dz}";
          if (_chunks.TryGetValue(key, out var chunk))
          {
            _chunks.Remove(key);
            if (chunk != null)
            {
              Destroy(chunk.gameObject);
            }

            yield return null;
          }
        }
      }

      _chunkX = chunkX;
      _chunkZ = chunkZ;
      
      _loading = false;
      Debug.Log("Loaded!");
    }
    
    private void Start()
    {
      provider.Generate(64, 64, chunkHeight);
      StartCoroutine(LoadTerrain(0, 0));
    }
    
    private void Update()
    {
      if (_loading)
      {
        return;
      }

      var current = UnityEngine.Camera.main;

      if (current == null)
      {
        return;
      }
      
      var position = current.transform.position;
      var chunkX = (int)(position.x / chunkSize);
      var chunkZ = (int)(position.z / chunkSize);

      if (chunkX != _chunkX || chunkZ != _chunkZ)
      {
        StartCoroutine(LoadTerrain(chunkX, chunkZ));
      }
    }
  }
}