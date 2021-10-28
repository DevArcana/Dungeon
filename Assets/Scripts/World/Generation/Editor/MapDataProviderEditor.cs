using UnityEditor;
using UnityEngine;

namespace World.Generation.Editor
{
  [CustomEditor(typeof(MapDataProvider))]
  public class MapDataProviderEditor : UnityEditor.Editor
  {
    public override void OnInspectorGUI()
    {
      base.OnInspectorGUI();
      var dungeon = (MapDataProvider) target;

      if (GUILayout.Button("Generate"))
      {
        dungeon.Generate(true);
      }
    }
  }
}