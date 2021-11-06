using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EntityLogic.AI
{
    
    public class PersonalityProvider : MonoBehaviour
    {
        [Range(0.0f, 1.0f)]
        public float aggressiveness;
        
        [Range(0.0f, 1.0f)]
        public float teamwork;

        public Personality personality;

        private void Awake()
        {
            aggressiveness = Random.Range(personality.minAggressiveness, personality.maxAggressiveness);
            teamwork = Random.Range(personality.minTeamwork, personality.maxTeamwork);
        }
    }
}