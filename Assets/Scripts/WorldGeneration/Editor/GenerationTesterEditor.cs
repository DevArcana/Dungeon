using UnityEditor;
using UnityEngine;

namespace WorldGeneration.Editor
{
    [CustomEditor(typeof(GenerationTester))]
    public class GenerationTesterEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Generate"))
            {
                var tester = (GenerationTester) target;
                tester.Generate();
            }
        }
    }
}