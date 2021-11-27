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
                (ActionType.HealAlly, UtilityFunctions.HealAllyUtility(entity, out var healAllyTarget)),
                (ActionType.Retreat, UtilityFunctions.RetreatUtility(entity, coverMap, out var retreatTarget)),
                (ActionType.Fireball, UtilityFunctions.FireballUtility(entity, out var fireballTarget)),
                (ActionType.TacticalMovement, UtilityFunctions.TacticalMovementUtility(entity, targetEntity, coverMap, out var tacticalMoveTarget)),
            }.Where(x => x.Item2 > 0.04f).OrderByDescending(x => x.Item2).ToList();

            AILogs.AddMainLogEndl($"{entity.name}");
            var message = "";
            foreach (var (actionType, score) in utilities)
            {
                message += $"{actionType} - {score:F2}\n";
            }
            var result = Helpers.WeightedRandom(utilities);
            AILogs.AddMainLog($"Chosen action: {result},");
            AILogs.AddMainLog($"Points left: {TurnManager.instance.ActionPoints.ActionPoints},");
            AILogs.AddSecondaryLogEndl(message.Trim());

            return result switch
            {
                ActionType.Retreat => (result, retreatTarget),
                ActionType.ChargePlayer => (result, chargeTarget),
                ActionType.HealAlly => (result, healAllyTarget),
                ActionType.Fireball => (result, fireballTarget),
                ActionType.TacticalMovement => (result, tacticalMoveTarget),
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
                    abilityProcessor.SelectAbility<HealSelfAbility>();
                    var ability = abilityProcessor.SelectedAbility;
                    AILogs.AddMainLogEndl($"Cost of heal self: {ability.GetEffectiveCost(entity.GridPos)}");
                    if (abilityProcessor.CanExecute(entity.GridPos))
                    {
                        abilityProcessor.Execute(entity.GridPos);
                    }
                    else throw new Exception($"Ability {action} is not possible, but it was chosen for execution.");
                    break;
                }
                case ActionType.HealAlly:
                {
                    abilityProcessor.SelectAbility<HealAllyAbility>();
                    var ability = abilityProcessor.SelectedAbility;
                    AILogs.AddMainLogEndl($"Cost of heal ally: {ability.GetEffectiveCost((GridPos)target!)}");
                    if (abilityProcessor.CanExecute((GridPos)target!))
                    {
                        abilityProcessor.Execute((GridPos)target!);
                    }
                    else throw new Exception($"Ability {action} is not possible, but it was chosen for execution.");
                    break;
                }
                case ActionType.Fireball:
                {
                    abilityProcessor.SelectAbility<FireballAbility>();
                    var ability = abilityProcessor.SelectedAbility;
                    AILogs.AddMainLogEndl($"Cost of fireball: {ability.GetEffectiveCost((GridPos)target!)}");
                    if (abilityProcessor.CanExecute((GridPos)target!))
                    {
                        abilityProcessor.Execute((GridPos)target!);
                    }
                    else throw new Exception($"Ability {action} is not possible, but it was chosen for execution.");
                    break;
                }
                case ActionType.Retreat:
                case ActionType.ChargePlayer:
                case ActionType.TacticalMovement:
                {
                    var ability = abilityProcessor.SelectedAbility;
                    AILogs.AddMainLogEndl($"Cost of move: {ability.GetEffectiveCost((GridPos) target!)}");
                    if (abilityProcessor.CanExecute((GridPos)target!))
                    {
                        abilityProcessor.Execute((GridPos)target!);
                    }
                    else throw new Exception($"Ability {action} is not possible, but it was chosen for execution.");
                    break;
                }
                case ActionType.Pass:
                    TurnManager.instance.NextTurn();
                    break;
                default:
                    Debug.Log("No action found");
                    TurnManager.instance.NextTurn();
                    break;
            }
            
            AILogs.Log();
        }
    }
}