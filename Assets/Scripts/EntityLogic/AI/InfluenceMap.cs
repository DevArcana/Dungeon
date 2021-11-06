using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using World.Common;

namespace EntityLogic.AI
{
    public class InfluenceMap : MonoBehaviour
    {
        public InfluenceMap instance;
        
        private Dictionary<GridPos, Dictionary<GridLivingEntity, float>> _influencedPoints;
        private Dictionary<GridLivingEntity, List<GridPos>> _entityInfluence;
        private SerializableMap<float> _influenceMap;

        public InfluenceMap()
        {
            var map = World.World.instance;
            _influencedPoints = new Dictionary<GridPos, Dictionary<GridLivingEntity, float>>();
            _entityInfluence = new Dictionary<GridLivingEntity, List<GridPos>>();
            _influenceMap = new SerializableMap<float>(map.MapWidth, map.MapHeight);
        }
        
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
                return;
            }

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            _influencedPoints.Clear();
            _entityInfluence.Clear();
        }
    }
}