using UnityEngine;
using World.Mesh;

namespace World
{
  [RequireComponent(typeof(MeshRenderer))]
  [RequireComponent(typeof(MeshFilter))]
  public class WorldChunk : MonoBehaviour
  {
    public int size = 16;
    public Heightmap heightmap;

    public void Rebuild()
    {
      if (heightmap == null)
      {
        heightmap = GetComponentInParent<Heightmap>();
      }
      
      var filter = GetComponent<MeshFilter>();
      var cave = new CaveBuilder();

      var position = transform.position;
      var xOffset = (int) position.x;
      var zOffset = (int) position.z;

      for (var z = 0; z < size; z++)
      {
        for (var x = 0; x < size; x++)
        {
          var y = heightmap.GetHeightAt(x + xOffset, z + zOffset);
          cave.AddFloor(new Vector3(x, y, z), new Vector3(x + 1, y, z + 1));

          if (x == 0)
          {
            cave.AddWall(new Vector3(x, 0.0f, z + 1), new Vector3(x, y, z));
          }
          else if (x == size - 1)
          {
            cave.AddWall(new Vector3(x + 1, 0.0f, z), new Vector3(x + 1, y, z + 1));
          }

          if (z == 0)
          {
            cave.AddWall(new Vector3(x, 0.0f, z), new Vector3(x + 1, y, z));
          }
          else if (z == size - 1)
          {
            cave.AddWall(new Vector3(x + 1, 0.0f, z + 1), new Vector3(x, y, z + 1));
          }

          if (x < size - 1)
          {
            var h = heightmap.GetHeightAt(x + 1 + xOffset, z + zOffset);
            if (y != h)
            {
              cave.AddWall(new Vector3(x + 1, y, z + 1), new Vector3(x + 1, h, z));
            }
          }

          if (z < size - 1)
          {
            var h = heightmap.GetHeightAt(x + xOffset, z + 1 + zOffset);
            if (y != h)
            {
              cave.AddWall(new Vector3(x, y, z + 1), new Vector3(x + 1, h, z + 1));
            }
          }
        }
      }

      filter.mesh = cave.Build();
    }
  }
}