using UnityEditor;
using UnityEngine;

namespace World.Editor
{
    [CustomEditor(typeof(WorldDataProvider))]
    public class WorldDataProviderEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            if (GUILayout.Button("Generate"))
            {
                var provider = (WorldDataProvider) target;
                provider.Generate(64, 64, 4);
            }
        }
    }
}