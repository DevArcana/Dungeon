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
        public InfluenceMap instance;
        
        private Dictionary<GridPos, Dictionary<GridLivingEntity, int>> _influencedPoints;
        private Dictionary<GridLivingEntity, List<GridPos>> _entityInfluence;
        private SerializableMap<float> _influenceMap;

        public InfluenceMap()
        {
            var map = World.World.instance;
            _influencedPoints = new Dictionary<GridPos, Dictionary<GridLivingEntity, int>>();
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
                _influencedPoints[pos][entity] = (int) pathNode.gCost;
                CalculateInfluenceOnPos(pos);
            }

            _entityInfluence[entity] = pointsOfInfluence;
        }

        private void CalculateInfluenceOnPos(GridPos pos)
        {
            var sum = 0.0f;
            foreach (var point in _influencedPoints[pos])
            {
                var side = point.Key is EnemyEntity ? 1 : -1;
                var maxDistance = TurnManager.instance.CurrentTurnTaker == point.Key
                    ? TurnManager.instance.ActionPoints.ActionPoints
                    : ActionPointsProcessor.MaxActionPoints;
                sum += side * (1 - point.Value / maxDistance);
            }

            _influenceMap[pos.x, pos.y] = sum;
        }

        private void TurnEntityRemoved(object sender, TurnManager.TurnEventArgs e)
        {
            RemoveEntityInfluence(e.Entity);
        }

        private void TurnEntityAdded(object sender, TurnManager.TurnEventArgs e)
        {
            AddEntityInfluence(e.Entity);
        }

        private void OnDestroy()
        {
            TurnManager.instance.TurnEntityAdded -= TurnEntityAdded;
            TurnManager.instance.TurnEntityRemoved -= TurnEntityRemoved;
        }
        
    }
}