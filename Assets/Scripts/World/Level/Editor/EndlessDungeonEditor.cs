using UnityEditor;
using UnityEngine;

namespace World.Level.Editor
{
  [CustomEditor(typeof(LevelProvider))]
  public class EndlessDungeonEditor : UnityEditor.Editor
  {
    public override void OnInspectorGUI()
    {
      base.OnInspectorGUI();
      var dungeon = (LevelProvider) target;

      if (GUILayout.Button("Generate"))
      {
        dungeon.Generate();
      }
    }
  }
}