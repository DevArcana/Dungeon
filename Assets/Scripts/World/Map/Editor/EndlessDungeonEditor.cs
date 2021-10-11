using UnityEditor;
using UnityEngine;

namespace World.Map.Editor
{
  [CustomEditor(typeof(EndlessDungeon))]
  public class EndlessDungeonEditor : UnityEditor.Editor
  {
    public override void OnInspectorGUI()
    {
      base.OnInspectorGUI();
      var dungeon = (EndlessDungeon) target;

      if (GUILayout.Button("Generate"))
      {
        dungeon.Generate();
      }
    }
  }
}