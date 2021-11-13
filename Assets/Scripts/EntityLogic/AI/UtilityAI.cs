using System.Collections.Generic;
using System.Linq;
using TurnSystem;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EntityLogic.AI
{
    public class UtilityAI
    {
        private List<GridLivingEntity> _enemies;
        private List<GridLivingEntity> _players;
        
        public UtilityAI()
        {
            _enemies = new List<GridLivingEntity>();
            _players = new List<GridLivingEntity>();
            
            var entities = TurnManager.instance.PeekQueue();
            foreach (var entity in entities)
            {
                if (entity is PlayerEntity) _players.Add(entity);
                else if (entity is EnemyEntity) _enemies.Add(entity);
            }
        }
        
        private ActionType ChooseNextAction(EnemyEntity entity)
        {
            var targetEntity = Pathfinding.FindClosestPlayer(entity.GridPos);
            var utilities = new List<(ActionType, float)>
            {
                (ActionType.Attack, MeleeAttackUtility(entity, targetEntity)),
                (ActionType.Run, RushPlayerUtility(entity, targetEntity)),
                (ActionType.Pass, PassTurnUtility(entity))
            }.Where(x => x.Item2 != 0f).OrderByDescending(x => x.Item2).ToList();

            return WeightedRandom(utilities);
            
        }

        public void PerformNextAction(EnemyEntity entity)
        {
            var targetEntity = Pathfinding.FindClosestPlayer(entity.GridPos);

            switch (ChooseNextAction(entity))
            {
                case ActionType.Attack:
                {
                    var ability = entity.abilities.SelectedAbility;
                    TurnManager.instance.ActionPoints.ReservePoints(ability.GetEffectiveCost(targetEntity.GridPos));
                    TurnManager.instance.ActionPoints.SpendReservedPoints();
                    ability.Execute(targetEntity.GridPos);
                    return;
                }
                case ActionType.Run:
                {
                    var pathfinding = new Pathfinding();
                    var (path, _) = pathfinding.FindPath(entity.GridPos, targetEntity.GridPos);
                    var target = path[path.Count - 2];
                    var ability = entity.abilities.SelectedAbility;
                    TurnManager.instance.ActionPoints.ReservePoints(ability.GetEffectiveCost(target));
                    TurnManager.instance.ActionPoints.SpendReservedPoints();
                    ability.Execute(target);
                    return;
                }
                case ActionType.Pass:
                    TurnManager.instance.NextTurn();
                    return;
                default:
                    Debug.Log("No action found");
                    TurnManager.instance.NextTurn();
                    return;
            }
        }

        private static ActionType WeightedRandom(List<(ActionType, float)> utilities)
        {
            var (_, bestUtilityScore) = utilities.First();
            var bestUtilities = utilities
                .Where(x => x.Item2 >= bestUtilityScore * 0.9f)
                .ToList();
            var totalScore = 0f;

            foreach (var (_, score) in bestUtilities)
            {
                totalScore += score;
            }

            var choice = Random.Range(0f, totalScore);
            var accumulator = 0f;
            
            foreach (var (action, score) in bestUtilities)
            {
                accumulator += score;
                if (choice < accumulator) return action;
            }
            
            return ActionType.Pass;
        }

        private float MeleeAttackUtility(GridLivingEntity entity, GridLivingEntity targetEntity)
        {
            var availableActionPoints = TurnManager.instance.ActionPoints.ActionPoints;
            var pathfinding = new Pathfinding();
            if (entity.GridPos.OneDimDistance(targetEntity.GridPos) == 1
                && pathfinding.FindPath(entity.GridPos, targetEntity.GridPos, 1).Item2 == 1
                && availableActionPoints >= entity.abilities.SelectedAbility.GetEffectiveCost(targetEntity.GridPos))
            {
                return 1f;
            }
            return 0f;
        }

        private float RushPlayerUtility(GridLivingEntity entity, GridLivingEntity targetEntity)
        {
            if (entity.GridPos.OneDimDistance(targetEntity.GridPos) == 1) return 0f;
            var availableActionPoints = TurnManager.instance.ActionPoints.ActionPoints;
            var maxCost = availableActionPoints + 2;
            var cost = entity.abilities.SelectedAbility.GetEffectiveCost(targetEntity.GridPos);
            if (cost > maxCost) return 0f;
            return Mathf.Pow(cost / (float) maxCost, 0.333f);
        }

        private float PassTurnUtility(EnemyEntity entity)
        {
            return 0.05f;
        }
    }
}