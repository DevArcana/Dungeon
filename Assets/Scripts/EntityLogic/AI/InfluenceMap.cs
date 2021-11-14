using System.Collections.Generic;
using System.Linq;
using TurnSystem;
using Unity.Plastic.Newtonsoft.Json.Serialization;
using UnityEngine;
using World.Common;

namespace EntityLogic.AI
{
    public class InfluenceMap : MonoBehaviour
    {
        public static InfluenceMap instance;
        
        public Dictionary<GridPos, Dictionary<GridLivingEntity, int>> _influencedPoints;
        public Dictionary<GridLivingEntity, List<GridPos>> _entityInfluence;
        public Influence[,] _influenceMap;

        public InfluenceMap()
        {
            // var map = World.World.instance;
            _influencedPoints = new Dictionary<GridPos, Dictionary<GridLivingEntity, int>>();
            _entityInfluence = new Dictionary<GridLivingEntity, List<GridPos>>();
            // _influenceMap = new SerializableMap<float>(map.MapWidth, map.MapHeight);
            _influenceMap = new Influence[300, 300];
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

            TurnManager.instance.TurnEntityAdded += TurnEntityAdded;
            TurnManager.instance.TurnEntityRemoved += TurnEntityRemoved;
            TurnManager.instance.TurnChanged += TurnChanged;
        }

        private void RemoveEntityInfluence(GridLivingEntity entity)
        {
            if (!_entityInfluence.ContainsKey(entity)) return;
            var positions = _entityInfluence[entity];
            foreach (var position in positions)
            {
                var influence = _influencedPoints[position];
                influence.Remove(entity);
                _entityInfluence.Remove(entity);
                CalculateInfluenceOnPos(position);
            }
        }

        public void AddEntityInfluence(GridLivingEntity entity)
        {
            RemoveEntityInfluence(entity);
            var maxDistance = TurnManager.instance.CurrentTurnTaker == entity
                ? TurnManager.instance.ActionPoints.ActionPoints
                : ActionPointsProcessor.MaxActionPoints;
            var pathFinding = new Pathfinding();
            var shortestPathTree =
                pathFinding.GetShortestPathTree(entity.GridPos, maxDistance);

            var pointsOfInfluence = new List<GridPos>();
            
            foreach (var pathNode in shortestPathTree)
            {
                var pos = GridPos.At(pathNode.x, pathNode.y);
                pointsOfInfluence.Add(pos);
                if (!_influencedPoints.ContainsKey(pos))
                {
                    _influencedPoints[pos] = new Dictionary<GridLivingEntity, int>();
                }
                _influencedPoints[pos][entity] = (int) pathNode.gCost;
                CalculateInfluenceOnPos(pos);
            }

            _entityInfluence[entity] = pointsOfInfluence;
        }

        private void CalculateInfluenceOnPos(GridPos pos)
        {
            var playerInfluence = 0f;
            var enemyInfluence = 0f;
            foreach (var point in _influencedPoints[pos])
            {
                var maxDistance = TurnManager.instance.CurrentTurnTaker == point.Key
                    ? TurnManager.instance.ActionPoints.ActionPoints
                    : ActionPointsProcessor.MaxActionPoints;
                if (maxDistance == 0) continue;
                var influence = 1 - point.Value / (float)maxDistance;
                if (point.Key is EnemyEntity)
                {
                    enemyInfluence += influence;
                }
                else
                {
                    playerInfluence += influence;
                }
            }

            var overallInfluence = enemyInfluence - playerInfluence;

            _influenceMap[pos.x, pos.y] = new Influence(playerInfluence, enemyInfluence, overallInfluence);
        }
        
        public List<GridLivingEntity> GetInfluencersOnPos(GridPos pos)
        {
            return _influencedPoints.ContainsKey(pos) ? _influencedPoints[pos].Keys.ToList() : new List<GridLivingEntity>();
        }

        private void TurnEntityRemoved(object sender, TurnManager.TurnEventArgs e)
        {
            RemoveEntityInfluence(e.Entity);
        }

        private void TurnEntityAdded(object sender, TurnManager.TurnEventArgs e)
        {
            AddEntityInfluence(e.Entity);
        }
        
        private void TurnChanged(object sender, TurnManager.TurnEventArgs e)
        {
            if (e.PreviousEntity is null) return;
            AddEntityInfluence(e.PreviousEntity);
        }

        private void OnDestroy()
        {
            TurnManager.instance.TurnEntityAdded -= TurnEntityAdded;
            TurnManager.instance.TurnEntityRemoved -= TurnEntityRemoved;
            TurnManager.instance.TurnChanged -= TurnChanged;
        }
        
    }
}