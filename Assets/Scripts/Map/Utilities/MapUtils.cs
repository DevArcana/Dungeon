using Unity.Mathematics;
using UnityEngine;

namespace Map.Utilities
{
  public static class MapUtils
  {
    public static Vector3? ToWorldPos(MapPos pos)
    {
      // ReSharper disable once Unity.NoNullPropagation
      var height = WorldDataProvider.Instance?.GetHeightAt(pos);

      if (height is null)
      {
        return null;
      }

      return new Vector3(pos.x + 0.5f, height.Value, pos.y + 0.5f);
    }

    public static MapPos ToMapPos(Vector3 pos) => MapPos.At((int) math.floor(pos.x), (int) math.floor(pos.z));
  }
}