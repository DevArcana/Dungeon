using System;
using System.Collections.Generic;
using System.Linq;
using EntityLogic.Abilities;
using EntityLogic.Abilities.ReadyAbilities;
using TurnSystem;
using UnityEngine;
using World.Common;
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
        
        private (ActionType, GridPos?) ChooseNextAction(EnemyEntity entity)
        {
            var targetEntity = Pathfinding.FindClosestPlayer(entity.GridPos);
            var coverMap = new CoverMap(entity, InfluenceMap.instance.GetEntityInfluencedPos(entity), _players).GetCoverMap();
            var utilities = new List<(ActionType, float)>
            {
                (ActionType.ChargePlayer, UtilityFunctions.ChargePlayerUtility(entity, targetEntity, out var chargeTarget)),
                (ActionType.Pass, UtilityFunctions.PassTurnUtility()),
                (ActionType.HealSelf, UtilityFunctions.HealSelfUtility(entity)),
                (ActionType.Retreat, UtilityFunctions.RetreatUtility(entity, coverMap, out var retreatTarget))
            }.Where(x => x.Item2 > 0.04f).OrderByDescending(x => x.Item2).ToList();

            var message = $"{entity.name}\n";
            foreach (var (actionType, score) in utilities)
            {
                message += $"{actionType} - {score:F2}\n";
            }
            var result = WeightedRandom(utilities);
            message += $"Chosen action: {result}\nPoints left: {TurnManager.instance.ActionPoints.ActionPoints}";
            Debug.Log(message);

            return result switch
            {
                ActionType.Retreat => (result, retreatTarget),
                ActionType.ChargePlayer => (result, chargeTarget),
                _ => (result, null)
            };
        }

        public void PerformNextAction(EnemyEntity entity)
        {
            var abilityProcessor = AbilityProcessor.instance;
            abilityProcessor.DeselectAbility();

            var (action, target) = ChooseNextAction(entity);
            
            switch (action)
            {
                case ActionType.HealSelf:
                {
                    abilityProcessor.SelectAbility(typeof(HealSelfAbility));
                    var ability = abilityProcessor.SelectedAbility;
                    Debug.Log($"Cost of heal: {ability.GetEffectiveCost(entity.GridPos)}");
                    if (abilityProcessor.CanExecute(entity.GridPos))
                    {
                        abilityProcessor.Execute(entity.GridPos);
                    }
                    else throw new Exception($"Ability {action} is not possible, but it was chosen for execution.");
                    return;
                }
                case ActionType.Retreat:
                case ActionType.ChargePlayer:
                {
                    var ability = abilityProcessor.SelectedAbility;
                    Debug.Log($"Cost of move: {ability.GetEffectiveCost((GridPos) target!)}");
                    if (abilityProcessor.CanExecute((GridPos)target!))
                    {
                        abilityProcessor.Execute((GridPos)target!);
                    }
                    else throw new Exception($"Ability {action} is not possible, but it was chosen for execution.");
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