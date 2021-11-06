using UnityEngine;

namespace EntityLogic.AI
{
    [CreateAssetMenu(fileName = "Personality", menuName = "AI/Personality", order = 1)]
    public class Personality : ScriptableObject
    {
        public float minAggressiveness;
        
        public float maxAggressiveness;
        
        public float minTeamwork;
        
        public float maxTeamwork;
    }
}