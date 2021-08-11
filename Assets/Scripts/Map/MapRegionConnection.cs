namespace Map
{
  public class MapRegionConnection
  {
    public MapRegion RegionA { get; }
    public MapRegion RegionB { get; }
    
    public MapPos PosA { get; }
    public MapPos PosB { get; }

    public MapRegionConnection(MapRegion regionA, MapPos posA, MapRegion regionB, MapPos posB)
    {
      RegionA = regionA;
      RegionB = regionB;

      PosA = posA;
      PosB = posB;

      regionA.connectedRegions.Add(regionB);
      regionB.connectedRegions.Add(regionA);
    }
  }
}