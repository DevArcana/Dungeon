using System.Collections.Generic;
using System.Linq;
using EntityLogic.Abilities;
using TurnSystem;
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
            
            _influencedPoints = new Dictionary<GridPos, Dictionary<GridLivingEntity, int>>();
            _entityInfluence = new Dictionary<GridLivingEntity, List<GridPos>>();

            TurnManager.instance.TurnEntityAdded += TurnEntityAdded;
            TurnManager.instance.TurnEntityRemoved += TurnEntityRemoved;
            TurnManager.instance.TurnChanged += TurnChanged;
            AbilityProcessor.instance.AbilityFinishedExecution += AbilityFinishedExecution;
        }

        private void Start()
        {
            var map = World.World.instance;
            _influenceMap = new Influence[map.MapWidth, map.MapHeight];
        }

        private void RemoveEntityInfluence(GridLivingEntity entity)
        {
            if (!_entityInfluence.ContainsKey(entity)) return;
            var positions = _entityInfluence[entity];
            _entityInfluence.Remove(entity);
            foreach (var position in positions)
            {
                var influence = _influencedPoints[position];
                influence.Remove(entity);
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
                pathFinding.GetDictShortestPathTree(entity.GridPos, maxDistance);
            entity.pathTree = shortestPathTree;
            
            var pointsOfInfluence = new List<GridPos>();
            
            foreach (var element in shortestPathTree)
            {
                var (pos, pathNode) = (element.Key, element.Value);
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
        
        public HashSet<GridLivingEntity> GetInfluencersOnPos(GridPos pos)
        {
            return _influencedPoints.ContainsKey(pos) ? new HashSet<GridLivingEntity>(_influencedPoints[pos].Keys) : new HashSet<GridLivingEntity>();
        }

        private void TurnEntityRemoved(object sender, TurnManager.TurnEventArgs e)
        {
            RemoveEntityInfluence(e.Entity);
            var influencers = GetInfluencersOnPos(e.Entity.GridPos);
            foreach (var influencer in influencers)
            {
                AddEntityInfluence(influencer);
            }
        }

        private void TurnEntityAdded(object sender, TurnManager.TurnEventArgs e)
        {
            var influencers = GetInfluencersOnPos(e.Entity.GridPos);
            influencers.Add(e.Entity);
            foreach (var influencer in influencers)
            {
                AddEntityInfluence(influencer);
            }
        }
        
        private void TurnChanged(object sender, TurnManager.TurnEventArgs e)
        {
            if (e.PreviousEntity is null) return;
            AddEntityInfluence(e.PreviousEntity);
        }
        
        private void AbilityFinishedExecution()
        {
            var entity = TurnManager.instance.CurrentTurnTaker;
            if (TurnManager.instance.ActionPoints.ActionPoints == ActionPointsProcessor.MaxActionPoints) return;
            if (_influencedPoints[entity.GridPos][entity] == 1)
            {
                AddEntityInfluence(TurnManager.instance.CurrentTurnTaker);
            }
            else
            {
                var previousPos = GetInfluencedPosOfCost(entity, 0).ToList()[0];
                var influencers = GetInfluencersOnPos(entity.GridPos);
                influencers.UnionWith(GetInfluencersOnPos(previousPos));
                foreach (var influencer in influencers)
                {
                    AddEntityInfluence(influencer);
                }
            }
        }

        private void OnDestroy()
        {
            TurnManager.instance.TurnEntityAdded -= TurnEntityAdded;
            TurnManager.instance.TurnEntityRemoved -= TurnEntityRemoved;
            TurnManager.instance.TurnChanged -= TurnChanged;
        }

        public Influence GetInfluenceOnPos(GridPos entityGridPos)
        {
            return _influenceMap[entityGridPos.x, entityGridPos.y];
        }

        public float GetEntityInfluenceOnPos(GridLivingEntity entity, GridPos pos)
        {
            if (!_influencedPoints.ContainsKey(pos) ||
                !_influencedPoints[pos].ContainsKey(entity))
            {
                return 0;
            }
            return 1 - _influencedPoints[pos][entity] / (float) TurnManager.instance.ActionPoints.ActionPoints;
        }

        public List<GridPos> GetEntityInfluencedPos(GridLivingEntity entity)
        {
            return !_entityInfluence.ContainsKey(entity) ? new List<GridPos>() : _entityInfluence[entity];
        }

        public HashSet<GridPos> GetInfluencedPosOfCost(GridLivingEntity entity, int cost)
        {
            var influencedPosOfCost = new HashSet<GridPos>();
            if (!_entityInfluence.ContainsKey(entity))
            {
                return influencedPosOfCost;
            }

            var influencedPos = _entityInfluence[entity];

            foreach (var pos in influencedPos.Where(pos => _influencedPoints[pos][entity] == cost))
            {
                influencedPosOfCost.Add(pos);
            }

            return influencedPosOfCost;
        }
    }
}
