using System.Collections.Generic;
using System.Linq;
using TurnSystem;
using UnityEngine;
using World.Common;

namespace EntityLogic.AI.Bucketing
{
    public class DefensiveBucket : IBucket
    {
        public float Score { get; }

        public DefensiveBucket(EnemyEntity entity)
        {
            if (entity.currentTurnActions.Any(x => OffensiveBucket.GetActions().Any(y => y == x)))
            {
                AILogs.AddSecondaryLogEndl("Defensive bucket is not available.");
                return;
            }
            
            var influenceMap = InfluenceMap.instance;

            // -0.25 to 0.25
            var aggressivenessFactor = (0.5f - entity.aggressiveness) * 0.5f;
            
            var health = entity.health;
            var healthPercentage = health.Health / health.MaximumHealth;
            var healthFactor = 1 - 1 / (1f + Mathf.Pow(2.718f * 1.2f, -(healthPercentage * 12) + 5.5f));
            
            var playerInfluence = influenceMap.GetInfluenceOnPos(entity.GridPos).playersInfluence;
            var threat = 1 / (1 + Mathf.Pow(2.718f * 1.2f, -(playerInfluence * 12) + 7f));

            var result = Mathf.Min(Mathf.Max(healthFactor * threat + aggressivenessFactor, 0), 1);
            AILogs.AddSecondaryLogEndl($"Defensive bucket score: {result:F2}");
            Score = result;
        }
        
        public (ActionType, GridPos?) EvaluateBucketActions(EnemyEntity entity)
        {
            var entities = TurnManager.instance.PeekQueue();
            var players = entities.OfType<PlayerEntity>().Select(queueEntity => entity).Cast<GridLivingEntity>().ToList();
            var coverMap = new CoverMap(entity, InfluenceMap.instance.GetEntityInfluencedPos(entity), players).GetCoverMap();

            var utilities = new List<(ActionType, float)>
            {
                (ActionType.HealSelf, UtilityFunctions.HealSelfUtility(entity)),
                (ActionType.Retreat, UtilityFunctions.RetreatUtility(entity, coverMap, out var retreatTarget)),
                (ActionType.Pass, UtilityFunctions.PassTurnUtility())
            }.Where(x => x.Item2 > 0.04f).OrderByDescending(x => x.Item2).ToList();

            var result = Helpers.WeightedRandom(utilities);
            
            return result switch
            {
                ActionType.Retreat => (result, retreatTarget),
                _ => (result, null)
            };
        }

        public static List<ActionType> GetActions()
        {
            return new List<ActionType>
            {
                ActionType.HealSelf,
                ActionType.HealAlly
            };
        }
    }
}