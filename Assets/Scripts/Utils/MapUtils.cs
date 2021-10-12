using Unity.Mathematics;
using UnityEngine;
using World.Common;

namespace Utils
{
  public static class MapUtils
  {
    public static Vector3 ToWorldPos(GridPos pos)
    {
      var height = World.World.instance.GetHeightAt(pos);
      return new Vector3(pos.x, height, pos.y);
    }

    public static GridPos ToMapPos(Vector3 pos) => GridPos.At((int) math.round(pos.x), (int) math.round(pos.z));
  }
}