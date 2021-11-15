using System.Collections.Generic;
using System.Linq;
using EntityLogic.Abilities;
using EntityLogic.Abilities.ReadyAbilities;
using TurnSystem;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EntityLogic.AI
{
    public class UtilityAI
    {
        private List<GridLivingEntity> _agents;
        private List<GridLivingEntity> _players;
        private InfluenceMap _influenceMap;
        
        public UtilityAI()
        {
            _agents = new List<GridLivingEntity>();
            _players = new List<GridLivingEntity>();
            _influenceMap = InfluenceMap.instance;
            
            var entities = TurnManager.instance.PeekQueue();
            foreach (var entity in entities)
            {
                if (entity is PlayerEntity) _players.Add(entity);
                else if (entity is EnemyEntity) _agents.Add(entity);
            }
        }
        
        private ActionType ChooseNextAction(EnemyEntity entity)
        {
            var targetEntity = Pathfinding.FindClosestPlayer(entity.GridPos);
            // var coverMap = new CoverMap(entity, InfluenceMap.instance.GetEntityInfluencedPos(entity), _players);
            var utilities = new List<(ActionType, float)>
            {
                (ActionType.MeleeAttack, UtilityFunctions.MeleeAttackUtility(entity, targetEntity)),
                (ActionType.RushPlayer, UtilityFunctions.RushPlayerUtility(entity, targetEntity)),
                (ActionType.Pass, UtilityFunctions.PassTurnUtility()),
                (ActionType.HealSelf, UtilityFunctions.HealSelfUtility(entity))
            }.Where(x => x.Item2 != 0f).OrderByDescending(x => x.Item2).ToList();

            return WeightedRandom(utilities);
            
        }

        public void PerformNextAction(EnemyEntity entity)
        {
            var abilityProcessor = AbilityProcessor.instance;
            
            var targetEntity = Pathfinding.FindClosestPlayer(entity.GridPos);

            switch (ChooseNextAction(entity))
            {
                case ActionType.MeleeAttack:
                {
                    var ability = abilityProcessor.SelectedAbility;
                    TurnManager.instance.ActionPoints.ReservePoints(ability.GetEffectiveCost(targetEntity.GridPos));
                    TurnManager.instance.ActionPoints.SpendReservedPoints();
                    ability.Execute(targetEntity.GridPos);
                    return;
                }
                case ActionType.RushPlayer:
                {
                    var pathfinding = new Pathfinding();
                    var (path, _) = pathfinding.FindPath(entity.GridPos, targetEntity.GridPos);
                    var target = path[path.Count - 2];
                    var ability = abilityProcessor.SelectedAbility;
                    TurnManager.instance.ActionPoints.ReservePoints(ability.GetEffectiveCost(target));
                    TurnManager.instance.ActionPoints.SpendReservedPoints();
                    ability.Execute(target);
                    return;
                }
                case ActionType.HealSelf:
                {
                    abilityProcessor.SelectAbility(typeof(HealSelfAbility));
                    var ability = abilityProcessor.SelectedAbility;
                    TurnManager.instance.ActionPoints.ReservePoints(ability.GetEffectiveCost(entity.GridPos));
                    TurnManager.instance.ActionPoints.SpendReservedPoints();
                    ability.Execute(entity.GridPos);
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
    }
}