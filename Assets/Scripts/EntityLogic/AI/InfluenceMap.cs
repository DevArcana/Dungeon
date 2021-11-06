using System;
using System.Collections.Generic;
using TurnSystem;
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
        private List<GridLivingEntity> _enemies;
        private List<GridLivingEntity> _players;

        public InfluenceMap()
        {
            _enemies = new List<GridLivingEntity>();
            _players = new List<GridLivingEntity>();
            
            var entities = TurnManager.instance.PeekQueue();
            foreach (var entity in entities)
            {
                if (entity is PlayerEntity) _players.Add(entity);
                else if (entity is EnemyEntity) _enemies.Add(entity);
            }
            
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
            TurnManager.instance.TurnEntityAdded += TurnEntityAdded;
            TurnManager.instance.TurnEntityRemoved += TurnEntityRemoved;
        }

        private void RemoveEntityInfluence(GridLivingEntity entity)
        {
            var positions = _entityInfluence[entity];
            foreach (var position in positions)
            {
                var influence = _influencedPoints[position];
                influence.Remove(entity);
                _entityInfluence.Remove(entity);
                CalculateInfluenceOnPos(position);
            }
        }

        private void AddEntityInfluence(GridLivingEntity entity)
        {
            
        }

        private void CalculateInfluenceOnPos(GridPos pos)
        {
            
        }
        
        private void TurnEntityRemoved(object sender, TurnManager.TurnEventArgs e)
        {
            RemoveEntityInfluence(e.Entity);
        }

        private void TurnEntityAdded(object sender, TurnManager.TurnEventArgs e)
        {
            AddEntityInfluence(e.Entity);
        }

        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            _influencedPoints.Clear();
            _entityInfluence.Clear();
            _influenceMap.Clear();
            InitializeMap();
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            TurnManager.instance.TurnEntityAdded -= TurnEntityAdded;
            TurnManager.instance.TurnEntityRemoved -= TurnEntityRemoved;
        }

        private void InitializeMap()
        {
            throw new System.NotImplementedException();
        }
    }
}