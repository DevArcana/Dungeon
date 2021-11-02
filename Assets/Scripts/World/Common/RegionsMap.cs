using System;

namespace World.Common
{
  [Serializable]
  public class RegionsMap : SerializableMap<int>
  {
    public RegionsMap(int width, int height) : base(width, height)
    {
    }
  }
}