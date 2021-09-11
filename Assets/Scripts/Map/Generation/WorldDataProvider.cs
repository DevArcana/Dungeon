using Map.Generation;
using UnityEngine;

public class WorldDataProvider : MonoBehaviour
{
  public static WorldDataProvider Instance { get; private set; }

  public MapGenerator generator;

  private void Awake()
  {
    if (Instance != null)
    {
      Debug.LogWarning("Multiple world data providers in the scene!");
    }

    Instance = this;
  }
}
