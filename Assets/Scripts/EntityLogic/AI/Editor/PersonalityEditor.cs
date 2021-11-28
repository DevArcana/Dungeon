using UnityEditor;

namespace EntityLogic.AI.Editor
{
    [CustomEditor(typeof(Personality))]
    public class PersonalityEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var personality = (Personality) target;
            EditorGUILayout.LabelField($"Aggressiveness: {personality.minAggressiveness} - {personality.maxAggressiveness}");
            EditorGUILayout.MinMaxSlider(ref personality.minAggressiveness, ref personality.maxAggressiveness, 0.0f, 1.0f);
            personality.minAggressiveness = EditorGUILayout.FloatField(personality.minAggressiveness);
            personality.maxAggressiveness = EditorGUILayout.FloatField(personality.maxAggressiveness);
            
            EditorGUILayout.LabelField($"Teamwork: {personality.minTeamwork} - {personality.maxTeamwork}");
            EditorGUILayout.MinMaxSlider(ref personality.minTeamwork, ref personality.maxTeamwork, 0.0f, 1.0f);
            personality.minTeamwork = EditorGUILayout.FloatField(personality.minTeamwork);
            personality.maxTeamwork = EditorGUILayout.FloatField(personality.maxTeamwork);
            
            personality.isRanged = EditorGUILayout.ToggleLeft("Is Ranged", personality.isRanged);
        }
    }
}
