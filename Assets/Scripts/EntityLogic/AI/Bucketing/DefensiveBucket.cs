using System.Collections.Generic;
using System.Linq;
using TurnSystem;
using World.Common;

namespace EntityLogic.AI.Bucketing
{
    public class DefensiveBucket : IBucket
    {
        public float EvaluateBucketUtility(EnemyEntity entity)
        {
            throw new System.NotImplementedException();
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
    }
}