using System;
using UnityEngine;
using World.Common;

namespace World.Level.Generation
{
  [Serializable]
  public class MapRegionConnection
  {
    [SerializeField]
    private MapRegion regionA;
    
    [SerializeField]
    private MapRegion regionB;
    
    [SerializeField]
    private GridPos posA;
    
    [SerializeField]
    private GridPos posB;

    public MapRegion RegionA => regionA;

    public MapRegion RegionB => regionB;

    public GridPos PosA => posA;

    public GridPos PosB => posB;

    public MapRegionConnection(MapRegion regionA, GridPos posA, MapRegion regionB, GridPos posB)
    {
      this.regionA = regionA;
      this.regionB = regionB;

      this.posA = posA;
      this.posB = posB;

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