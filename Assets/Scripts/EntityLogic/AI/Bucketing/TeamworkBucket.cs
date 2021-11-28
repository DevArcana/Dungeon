using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using World.Common;

namespace EntityLogic.AI.Bucketing
{
    public class TeamworkBucket : IBucket
    {
        public float EvaluateBucketUtility(EnemyEntity entity)
        {
            var teamworkFactor = (entity.teamwork - 0.5f) * 0.5f;
            
            var result = Mathf.Min(Mathf.Max(UtilityFunctions.HealAllyUtility(entity, out _) + teamworkFactor, 0), 1);
            AILogs.AddSecondaryLogEndl($"Teamwork bucket score: {result:F2}");
            return result;
        }

        public (ActionType, GridPos?) EvaluateBucketActions(EnemyEntity entity)
        {
            var score = UtilityFunctions.HealAllyUtility(entity, out var healAllyTarget);

            if (score > 0.04f)
            {
                return (ActionType.HealAlly, healAllyTarget);
            }
            return (ActionType.Pass, null);
        }
    }
}