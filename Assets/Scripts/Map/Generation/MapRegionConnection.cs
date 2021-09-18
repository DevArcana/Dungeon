using Grid;

namespace Map.Generation
{
  public class MapRegionConnection
  {
    public MapRegion RegionA { get; }
    public MapRegion RegionB { get; }
    
    public GridPos PosA { get; }
    public GridPos PosB { get; }

    public MapRegionConnection(MapRegion regionA, GridPos posA, MapRegion regionB, GridPos posB)
    {
      RegionA = regionA;
      RegionB = regionB;

      PosA = posA;
      PosB = posB;

      regionA.connectedRegions.Add(regionB);
      regionB.connectedRegions.Add(regionA);

      if (regionA.IsConnectedToRoot && !regionB.IsConnectedToRoot)
      {
        regionB.ConnectToRoot();
      }
      
      if (!regionA.IsConnectedToRoot && regionB.IsConnectedToRoot)
      {
        regionA.ConnectToRoot();
      }
    }
  }
}