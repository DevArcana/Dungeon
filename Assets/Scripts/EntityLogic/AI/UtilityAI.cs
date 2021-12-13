using System;
using System.Collections.Generic;
using System.Linq;
using EntityLogic.Abilities;
using EntityLogic.Abilities.ReadyAbilities;
using EntityLogic.AI.Bucketing;
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

        private static (ActionType, GridPos?) ChooseNextAction(EnemyEntity entity)
        {
            int nameStart, nameLength;
            
            var buckets = new List<IBucket>
                {
                    new OffensiveBucket(entity),
                    new DefensiveBucket(entity),
                    new TeamworkBucket(entity)
                }.Where(bucket => bucket.Score > 0.04f)
                .OrderByDescending(bucket => bucket.Score);

            foreach (var bucket in buckets)
            {
                var (action, target) = bucket.EvaluateBucketActions(entity);
                if (action == ActionType.Pass) continue;
                nameStart = entity.name.IndexOf('(') + 1;
                nameLength = entity.name.IndexOf(')') - nameStart;
                AILogs.AddMainLogEndl($"{entity.name.Substring(nameStart, nameLength)}, " +
                                      $"HP: {entity.health.Health}/{entity.health.MaximumHealth}");
                AILogs.AddMainLog($"Chosen action: {action},");
                AILogs.AddMainLog($"Points left: {TurnManager.instance.ActionPoints.ActionPoints},");
                return (action, target);
            }

            nameStart = entity.name.IndexOf('(') + 1;
            nameLength = entity.name.IndexOf(')') - nameStart;
            AILogs.AddMainLogEndl($"{entity.name.Substring(nameStart, nameLength)}, " +
                                  $"HP: {entity.health.Health}/{entity.health.MaximumHealth}");
            AILogs.AddMainLog($"Chosen action: {ActionType.Pass},");
            AILogs.AddMainLog($"Points left: {TurnManager.instance.ActionPoints.ActionPoints},");
            return (ActionType.Pass, null);
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
            
            entity.currentTurnActions.Add(action);
            
            AILogs.AdjustMainLog();
            LogConsole.Log(AILogs.GetMainLog());
            AILogs.Log();
        }
    }
}
