using UnityEngine;

namespace World.Map.Mesh
{
  public static class SpaceDistortion
  {
    private const float Vertical = 0.1f;
    private const float Horizontal = 0.2f;
    
    public static Vector3 Distort(Vector3 point, Vector3 offset)
    {
      return point;
    }

    // public static Vector3 Distort(Vector3 point, Vector3 offset)
    // {
    //   var key = point + offset;
    //   var seed = (int) math.round(key.x) * 13 + (int) math.round(key.y) * 7 + (int) math.round(key.z) * 23;
    //   Random.InitState(seed);
    //
    //   var offsetX = Random.Range(-Horizontal, Horizontal);
    //   var offsetZ = Random.Range(-Horizontal, Horizontal);
    //   var offsetY = Random.Range(-Vertical, Vertical);
    //
    //   return point + Vector3.right * offsetX + Vector3.forward * offsetZ + Vector3.up * offsetY;
    // }
  }
}