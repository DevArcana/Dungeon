using UnityEditor;
using UnityEngine;

namespace World.Editor
{
    [CustomEditor(typeof(WorldChunkManager))]
    public class WorldChunkManagerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            if (GUILayout.Button("Rebuild"))
            {
                var cave = (WorldChunkManager) target;
                cave.editMode = true;
                cave.Rebuild();
            }
        }
    }
}