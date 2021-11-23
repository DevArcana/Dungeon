using System.Collections.Generic;
using System.Linq;
using World.Common;

namespace EntityLogic.AI.Bucketing
{
    public class OffensiveBucket : IBucket
    {
        public float EvaluateBucketUtility(EnemyEntity entity)
        {
            throw new System.NotImplementedException();
        }

        public (ActionType, GridPos?) EvaluateBucketActions(EnemyEntity entity)
        {
            var targetEntity = Pathfinding.FindClosestPlayer(entity.GridPos);

            var utilities = new List<(ActionType, float)>
            {
                (ActionType.ChargePlayer, UtilityFunctions.ChargePlayerUtility(entity, targetEntity, out var chargeTarget)),
                (ActionType.Pass, UtilityFunctions.PassTurnUtility())
            }.Where(x => x.Item2 > 0.04f).OrderByDescending(x => x.Item2).ToList();

            var result = Helpers.WeightedRandom(utilities);
            
            return result switch
            {
                ActionType.ChargePlayer => (result, chargeTarget),
                _ => (result, null)
            };
        }
    }
}