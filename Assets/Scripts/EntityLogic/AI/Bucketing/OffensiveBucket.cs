using System.Collections.Generic;
using System.Linq;
using TurnSystem;
using UnityEngine;
using World.Common;

namespace EntityLogic.AI.Bucketing
{
    public class OffensiveBucket : IBucket
    {
        public float Score { get; }

        public OffensiveBucket(EnemyEntity entity)
        {
            if (entity.currentTurnActions.Contains(ActionType.Retreat))
            {
                AILogs.AddSecondaryLogEndl("Offensive bucket is not available.");
                return;
            }
            
            var aggressivenessFactor = (entity.aggressiveness - 0.5f) * 0.5f;
            
            var health = entity.health;
            var healthPercentage = health.Health / health.MaximumHealth;
            var healthFactor = 1 / (1f + Mathf.Pow(2.718f * 1.2f, -(healthPercentage * 12) + 5.5f));

            var map = World.World.instance;
            var targetEntity = Pathfinding.FindClosestPlayer(entity.GridPos);
            var playerHealth = map.GetOccupant(targetEntity.GridPos).health;
            var playerHealthPercentage = playerHealth.Health / playerHealth.MaximumHealth;
            var playerHealthFactor = Mathf.Min(Mathf.Pow(0.5f, playerHealthPercentage * 5f) + 0.7f, 1f);
            
            var result = Mathf.Min(Mathf.Max(healthFactor * playerHealthFactor + aggressivenessFactor, 0), 1);
            AILogs.AddSecondaryLogEndl($"Offensive bucket score: {result:F2}");
            Score = result;
        }

        public (ActionType, GridPos?) EvaluateBucketActions(EnemyEntity entity)
        {
            var targetEntity = Pathfinding.FindClosestPlayer(entity.GridPos);
            var entities = TurnManager.instance.PeekQueue();
            var players = entities.Where(x => x is PlayerEntity).ToList();
            var coverMap = new CoverMap(entity, InfluenceMap.instance.GetEntityInfluencedPos(entity), players).GetCoverMap();
            
            var utilities = new List<(ActionType, float)>
            {
                (ActionType.ChargePlayer, UtilityFunctions.ChargePlayerUtility(entity, targetEntity, out var chargeTarget)),
                (ActionType.Fireball, UtilityFunctions.FireballUtility(entity, out var fireballTarget)),
                (ActionType.TacticalMovement, UtilityFunctions.TacticalMovementUtility(entity, targetEntity, coverMap, out var tacticalMoveTarget)),
                (ActionType.Pass, UtilityFunctions.PassTurnUtility())
            }.Where(x => x.Item2 > 0.04f).OrderByDescending(x => x.Item2).ToList();

            var message = "";
            foreach (var (actionType, score) in utilities)
            {
                message += $"{actionType} - {score:F2}\n";
            }

            var result = Helpers.WeightedRandom(utilities);
            AILogs.AddSecondaryLogEndl(message.Trim());
            
            return result switch
            {
                ActionType.ChargePlayer => (result, chargeTarget),
                ActionType.Fireball => (result, fireballTarget),
                ActionType.TacticalMovement => (result, tacticalMoveTarget),
                _ => (result, null)
            };
        }

        public static List<ActionType> GetActions()
        {
            return new List<ActionType>
            {
                ActionType.ChargePlayer,
                ActionType.Fireball,
                ActionType.TacticalMovement
            };
        }
    }
}
