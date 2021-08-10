using UnityEditor;
using UnityEngine;

namespace World.Editor
{
  [CustomEditor(typeof(Chunk))]
  public class ChunkEditor : UnityEditor.Editor
  {
    public override void OnInspectorGUI()
    {
      base.OnInspectorGUI();
            
      if (GUILayout.Button("Generate"))
      {
        var provider = (Chunk) target;
        provider.Rebuild();
      }
    }
  }
}