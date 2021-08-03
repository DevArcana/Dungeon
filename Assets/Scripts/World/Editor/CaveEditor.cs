using UnityEditor;
using UnityEngine;

namespace World.Editor
{
    [CustomEditor(typeof(Cave))]
    public class CaveEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            if (GUILayout.Button("Generate"))
            {
                var cave = (Cave) target;
                cave.Generate();
            }
        }
    }
}