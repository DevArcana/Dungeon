using System.Collections.Generic;
using System.Linq;
using World.Common;

namespace EntityLogic.AI.Bucketing
{
    public class TeamworkBucket : IBucket
    {
        public float EvaluateBucketUtility(EnemyEntity entity)
        {
            throw new System.NotImplementedException();
        }

        public (ActionType, GridPos?) EvaluateBucketActions(EnemyEntity entity)
        {
            var utilities = new List<(ActionType, float)>
            {
                (ActionType.HealAlly, UtilityFunctions.HealAllyUtility(entity, out var healAllyTarget)),
                (ActionType.Pass, UtilityFunctions.PassTurnUtility())
            }.Where(x => x.Item2 > 0.04f).OrderByDescending(x => x.Item2).ToList();

            var result = Helpers.WeightedRandom(utilities);
            
            return result switch
            {
                ActionType.HealAlly => (result, healAllyTarget),
                _ => (result, null)
            };
        }
    }
}